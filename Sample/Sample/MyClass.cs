using System;
using System.Diagnostics;
using NUnit.Framework;
using gdio.unity_api.v2;
using gdio.common.objects;

namespace DemoTest
{
    [TestFixture]
    public class UnitTest
    {
        /* These parameters can be used to override settings used to test when running from the NUnit command line
         * Example: mono ../../ext/nunit3-console.exe ./Sample.dll --testparam:Mode=standalone --testparam:pathToExe=/Users/user/Desktop/GameDriver_Demo.app/Contents/MacOS/GameDriver_Demo
         */
        public string testMode = TestContext.Parameters.Get("Mode", "IDE");
        public string testHost = TestContext.Parameters.Get("Host", "localhost");
        public string executablePath = TestContext.Parameters.Get("executablePath", @"C:\Users\user\Builds\UnityBuild.exe");
        public string pathToExe = TestContext.Parameters.Get("pathToExe", null); // replace null with the path to your executable as needed, or via the command line as shown above

        // Here we initialize the ApiClient
        ApiClient api;

        [OneTimeSetUp]
        public void Connect()
        {
            api = new ApiClient();

            try
            {
                // First we need to create an instance of the ApiClient

                // If an executable path was supplied, we will launch the standalone game
                if (executablePath != null && testMode == "standalone") // launches a standalone executable
                {
                    int PID = ApiClient.Launch(executablePath);
                    Console.WriteLine($"Launching standalone executable with PID: {PID}");
                    api.Wait(5000);
                    api.Connect(testHost, 19734, false, 30);
                }
                else if (executablePath == null && testMode == "IDE") // connects to the IDE and starts Play mode
                {
                    api.Connect(testHost, 19734, true, 30);
                }
                else {
                    api.Connect(testHost, 19734, false, 30); // connects to an already running instance of the editor or standalone build, mobile device, etc...
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Enable input hooking
            api.EnableHooks(HookingObject.ALL);

            // Give the application some time to load. Time for your app may vary.
            api.Wait(3000);
        }

        [Test]
        public void Test1()
        {
            // Do something. Example:
            /// Wait for a button to exist
            /// api.WaitForObject("//*[@name='Button']", 30);
            /// Click the button
            /// api.ClickObject(MouseButtons.LEFT, "//*[@name='Button']", 30);
            /// api.Wait(1000);
            /// Check that some text appeared
            /// Assert.AreEqual("Success", api.GetObjectFieldValue<string>("//*[@name='Text']/fn:component('TMPro.TextMeshProUGUI')/@text"), "Text doesn't match");
        }

        [Test]
        public void Test2()
        {
            // Do something else. Tests should be able to run independently after the steps in [OneTimeSetup] and can use try/catch blocks to avoid exiting prematurely on failure
        }

        [OneTimeTearDown]
        public void Disconnect()
        {
            // Disconnect the GameDriver client from the agent
            api.DisableHooks(HookingObject.ALL);
            api.Wait(1000);
            api.Disconnect();
            api.Wait(1000);
            // Comment out this line if you want to keep the editor in Play mode
            api.StopEditorPlay();
        }
    }
}
