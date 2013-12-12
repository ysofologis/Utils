import requests
import base64
import json
import sys

PUBLIC_SERVICE_ADDRESS = "http://iapplyngprod.azurewebsites.net"
LOCAL_SERVICE_ADDRESS = "http://localhost:57611"
DEFAULT_SERVICE_ADDRESS = PUBLIC_SERVICE_ADDRESS
DEFAULT_PAGE_SIZE = 50

class Service:
    def __init__(self, service_address = DEFAULT_SERVICE_ADDRESS, user_name = "dev01", user_password = "PASSW0RD"):
        self.user_name = user_name
        self.user_password = user_password
        self.service_address = service_address
        self.session_id = None

    def get_session_id(self):
        if not self.session_id:
            auth_text = "%s:%s" % ( self.user_name, self.user_password )
            auth_header = { "Authorization" : "basic " + base64.b64encode(auth_text) }
            login_url = "%s/api/Sessions" % self.service_address
            login_resp = requests.post(url = login_url, headers = auth_header )
            if login_resp.status_code == 200:
                resp_data = login_resp.json()
                self.session_id = resp_data["SessionId"]

        return self.session_id

    def create_resource(self, resource_name, method_name = "", req_data = None ):
        request_url = "%s/api/%s/%s" % ( self.service_address, resource_name, method_name )
        headers =  { "session-id" : self.get_session_id(), 'Content-type': 'application/json' }
        response = requests.post(url = request_url, data = req_data, headers = headers)
        if response.status_code not in (200,201):
            response.raise_for_status()

    def update_resource(self, resource_name, resource_id, method_name = "", req_data = None ):
        request_url = "%s/api/%s/%s/%s" % ( self.service_address, resource_name, resource_id, method_name )
        headers =  { "session-id" : self.get_session_id(), 'Content-type': 'application/json' }
        response = requests.put(url = request_url, data = req_data, headers = headers)
        if response.status_code not in (200,201):
            response.raise_for_status()


    def _get_paged(self, resource_name, page = 1, page_size = DEFAULT_PAGE_SIZE):
        request_url = "%s/api/%s?page=%d&pagesize=%d" % ( self.service_address, resource_name, page, page_size )
        headers =  { "session-id" : self.get_session_id() }
        response = requests.get(url = request_url, headers = headers)
        if response.status_code == 200:
            request_items = response.json()["Items"]
            if len(request_items) > 0 and len(request_items) == page_size :
                next_items = self._get_paged(resource_name,page+1,page_size)
                return request_items + next_items
            else:
                return request_items
        else:
            response.raise_for_status()

    def get_resources(self, resource_name):
        return self._get_paged(resource_name)

    def get_resource(self, resource_name, resource_id):
        request_url = "%s/api/%s/%s" % ( self.service_address, resource_name, resource_id )
        headers =  { "session-id" : self.get_session_id() }
        response = requests.get(url = request_url, headers = headers)
        if response.status_code == 200:
            return response.json()
        else:
            response.raise_for_status()


    def upsert_specification(self, spec_resource, spec_name, spec_file, def_key = 'Definition'):
        f= open(spec_file,"r")
        def_text = f.read();
        def_node = json.loads(def_text)
        f.close()
        
        all_specs = self.get_resources(spec_resource)
        existing_id = None
        data = json.dumps( { 'Name': spec_name, def_key : def_node } )

        for p in all_specs:
            if p['Name'] == spec_name:
                existing_id = p['Id']
                break

        if existing_id:
            self.update_resource( resource_name = spec_resource, resource_id = existing_id, req_data = data)
        else:
            self.create_resource( resource_name = spec_resource, req_data = data)
        


def test1():
    try:
        iapply_service = Service()
        session_id = iapply_service.get_session_id()
        app_settings_all = iapply_service.get_resources("appsettings")
        app_settings_x = iapply_service.get_resource("appsettings", "cmis.productName")

        iapply_service.upsert_specification("casedefinitions", "WebServiceDemo", "C:\Projects\iApplyNG\scripts\december_2013_demo\emulated_clients\Case Definitions\WebServiceDemo.json")

        iapply_service.upsert_specification("taskspecifications", "WS_Demo_SelectStock", "C:\Projects\iApplyNG\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\WS_Demo_SelectStock.json", def_key = 'TaskDefinition')

    except Exception as x:
        print x


def create_demo_2013_specs():
    try:
        iapply_service = Service()
        iapply_service.upsert_specification("casedefinitions", "WebServiceDemo", "C:\Projects\iApplyNG\scripts\december_2013_demo\emulated_clients\Case Definitions\WebServiceDemo.json")
        iapply_service.upsert_specification("taskspecifications", "WS_Demo_SelectStock", "C:\Projects\iApplyNG\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\WS_Demo_SelectStock.json", def_key = 'TaskDefinition')
        iapply_service.upsert_specification("taskspecifications", "WS_Demo_ViewStock", "C:\Projects\iApplyNG\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\WS_Demo_ViewStock.json", def_key = 'TaskDefinition')
        print "OK"
    except Exception as x:
        print x

create_demo_2013_specs()