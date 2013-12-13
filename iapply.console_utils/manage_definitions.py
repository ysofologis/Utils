import json
import sys
from service_api import *

PROJECT_DIR = "C:\Projects\iApplyNG"

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


#------------------------------------------------------------------------------------------------
def create_web_service_demo_2013_specs(service_addr = DEFAULT_SERVICE_ADDRESS):
    try:
        iapply_service = Service(service_addr)
        iapply_service.upsert_specification("casedefinitions", "WebServiceDemo", "%s\scripts\december_2013_demo\emulated_clients\Case Definitions\WebServiceDemo.json" % PROJECT_DIR)
        iapply_service.upsert_specification("taskspecifications", "WS_Demo_SelectStock", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\WS_Demo_SelectStock.json" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)
        iapply_service.upsert_specification("taskspecifications", "WS_Demo_ViewStock", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\WS_Demo_ViewStock.json" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)
        print "OK"
    except Exception as x:
        print x
#------------------------------------------------------------------------------------------------
def create_parallel_demo_2013_specs(service_addr = DEFAULT_SERVICE_ADDRESS):
    try:
        iapply_service = Service(service_addr)
        case_id = iapply_service.upsert_specification("casedefinitions", "HumanTasksDemo_PD", "%s\scripts\december_2013_demo\emulated_clients\Case Definitions\HumanTaskDemo.txt" % PROJECT_DIR)

        iapply_service.upsert_specification( spec_resource = "subprocesses", spec_name = "RiskAnalysisSubProcess_PD", 
                                                spec_file = "%s\scripts\december_2013_demo\emulated_clients\Case Definitions\RiskSubProcess.txt" % PROJECT_DIR, 
                                                parent_id = case_id, 
                                                is_local = True)

        iapply_service.upsert_specification("taskspecifications", "ReviewApp_PD", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\BR_Manager_Task.txt" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)
        iapply_service.upsert_specification("taskspecifications", "CreateApplication_PD", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\BR_Teller_Task.txt" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)
        iapply_service.upsert_specification("taskspecifications", "AuditApp_PD", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\HQ_Auditor_Task.txt" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)
        iapply_service.upsert_specification("taskspecifications", "RiskAnalysisTask_PD", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\HQ_RiskAnalyst_Task.txt" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)
        iapply_service.upsert_specification("taskspecifications", "AdvancedRiskAnalysis_PD", "%s\Scripts\December_2013_demo\Emulated_Clients\Task Definitions\HQ_SeniorRiskAnalyst_Task.txt" % PROJECT_DIR, 
                                            def_key = 'TaskDefinition',
                                            naked_definition_on_update = False)

        print "<create_parallel_demo_2013_specs> --> OK"
    except Exception as x:
        print x

create_parallel_demo_2013_specs("http://localhost:57611")
create_web_service_demo_2013_specs("http://localhost:57611")