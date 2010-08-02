// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.3.2.0
//      Runtime Version:4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
namespace Orchard.Specs
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.3.2.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Module management")]
    public partial class ModuleManagementFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Modules.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Module management", "In order add and enable features\r\nAs a root Orchard system operator\r\nI want to in" +
                    "stall and enable modules and enable features", ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Installed modules are listed")]
        public virtual void InstalledModulesAreListed()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Installed modules are listed", ((string[])(null)));
#line 6
this.ScenarioSetup(scenarioInfo);
#line 7
testRunner.Given("I have installed Orchard");
#line 8
testRunner.When("I go to \"admin/modules\"");
#line 9
testRunner.Then("I should see \"<h1>Installed Modules</h1>\"");
#line 10
testRunner.And("I should see \"<h2>Themes\"");
#line 11
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Features of installed modules are listed")]
        public virtual void FeaturesOfInstalledModulesAreListed()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Features of installed modules are listed", ((string[])(null)));
#line 13
this.ScenarioSetup(scenarioInfo);
#line 14
testRunner.Given("I have installed Orchard");
#line 15
testRunner.When("I go to \"admin/modules/features\"");
#line 16
testRunner.Then("I should see \"<h1>Manage Features</h1>\"");
#line 17
testRunner.And("I should see \"<h3>Common</h3>\"");
#line 18
testRunner.And("the status should be 200 OK");
#line hidden
            testRunner.CollectScenarioErrors();
        }
    }
}
#endregion
