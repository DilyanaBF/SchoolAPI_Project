using AventStack.ExtentReports;
using SchoolAPI_TestProject.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Reqnroll;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Reqnroll;

namespace SchoolAPI_TestProject
{
    [Binding]
    public sealed class Hooks
    {

        private readonly ScenarioContext _scenarioContext;
        private IWebDriver _driver;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            ExtentManager.InitReport();
        }

        //[BeforeScenario("UI")]
        //public void BeforeUIScenario()
        //{
        //    var options = new ChromeOptions();
        //    options.AddArgument("--start-maximized"); // Отваряне на цял екран
        //    options.AddArgument("--disable-notifications"); // Деактивиране на изскачащи нотификации
        //
        //    _driver = new ChromeDriver(options);
        //
        //    // Запазване на WebDriver в контекста на сценариите
        //    _scenarioContext["WebDriver"] = _driver;
        //}
        //
        //[AfterScenario("UI")]
        //public void AfterUIScenario()
        //{
        //    if (_scenarioContext.ContainsKey("WebDriver"))
        //    {
        //        _driver = (IWebDriver)_scenarioContext["WebDriver"];
        //
        //        if (_scenarioContext.TestError != null) // Ако има грешка в теста
        //        {
        //            TakeScreenshot(_scenarioContext.ScenarioInfo.Title);
        //        }
        //
        //        _driver.Quit();
        //    }
        //}
        //
        ////[BeforeScenario("@tag1", Order =-9)]
        //[BeforeScenario("@tag1")]
        //public void BeforeScenarioWithTag()
        //{
        //    Console.WriteLine("Do something only when scenario is tagged with '@tag1'");
        //}
        //
        //[BeforeScenario(Order = 1)]
        //public void FirstBeforeScenario()
        //{
        //    Console.WriteLine("Do something before every scenario, the lowest order is executed first");
        //}
        //
        //[BeforeScenario(Order = -99)]
        //public void NegativeBeforeScenario()
        //{
        //    Console.WriteLine("FIRST FIRST BEFORE");
        //}

        [BeforeScenario]
        public void BeforeScenario()
        {
            var еxtentTest = ExtentManager.CreateTest("Scenario: " + GetScenarioName(_scenarioContext));
            _scenarioContext["ExtentTest"] = еxtentTest;
            string scenarioName = GetScenarioName(_scenarioContext);
            //UtilitiesMethods.LogMessage("Starting scenario", _scenarioContext);
            Logger.Log.Info("Start executing scenario:" + scenarioName);
        }

        [BeforeStep]
        public void BeforeStep()
        {
            UtilitiesMethods.LogMessage("Start executing Step: " + _scenarioContext.StepContext.StepInfo.Text, _scenarioContext, LogStatuses.Info);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Logger.Log.Info("Scenario finished.");
        }

        [AfterTestRun]
        public static void FlushReport()
        {
            ExtentManager.FlushReport();
        }

        private static string GetScenarioNameDELETE(ScenarioContext context)
        {
            string scenarioParameters = string.Empty;

            foreach (DictionaryEntry entry in context.ScenarioInfo.Arguments)
            {
                scenarioParameters += @"""" + entry.Value.ToString() + @"""" + ", ";
            }

            if (!string.IsNullOrEmpty(scenarioParameters))
            {
                scenarioParameters = "(" + scenarioParameters + ")";
            }

            string scenarioName = context.ScenarioInfo.Title + scenarioParameters;

            return scenarioName;
        }
        public static string GetScenarioName(ScenarioContext context)
        {
            var argumentPairs = new List<string>();

            foreach (DictionaryEntry entry in context.ScenarioInfo.Arguments)
            {
                if (entry.Key != null && entry.Value != null)
                {
                    argumentPairs.Add($"{entry.Key}=\"{entry.Value}\"");
                }
            }

            string argumentsPart = argumentPairs.Count > 0
                ? $" ({string.Join(", ", argumentPairs)})"
                : string.Empty;

            return context.ScenarioInfo.Title + argumentsPart;
        }
    }

}