using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlaywrightSharp.Tests.Attributes;
using PlaywrightSharp.Tests.BaseTests;
using PlaywrightSharp.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PlaywrightSharp.Tests.Chromium
{
    ///<playwright-file>chromium/chromium.spec.js</playwright-file>
    ///<playwright-describe>JSCoverage</playwright-describe>
    [Collection(TestConstants.TestFixtureBrowserCollectionName)]
    public class JSCoverageTests : PlaywrightSharpPageBaseTest
    {
        /// <inheritdoc/>
        public JSCoverageTests(ITestOutputHelper output) : base(output)
        {
        }

        ///<playwright-file>chromium/chromium.spec.js</playwright-file>
        ///<playwright-describe>JSCoverage</playwright-describe>
        ///<playwright-it>should work</playwright-it>
        [SkipBrowserAndPlatformFact(skipFirefox: true, skipWebkit: true)]
        public async Task ShouldWork()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/simple.html", LifecycleEvent.Networkidle);
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            Assert.Contains("/jscoverage/simple.html", coverage[0].Url);
            Assert.Equal(1, coverage[0].Functions.FirstOrDefault(f => f.FunctionName == "foo").Ranges[0].Count);
        }

        ///<playwright-file>chromium/chromium.spec.js</playwright-file>
        ///<playwright-describe>JSCoverage</playwright-describe>
        ///<playwright-it>should report sourceURLs</playwright-it>
        [SkipBrowserAndPlatformFact(skipFirefox: true, skipWebkit: true)]
        public async Task ShouldReportSourceUrls()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/sourceurl.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
            Assert.Equal("nicename.js", coverage[0].Url);
        }

        ///<playwright-file>chromium/chromium.spec.js</playwright-file>
        ///<playwright-describe>JSCoverage</playwright-describe>
        ///<playwright-it>should ignore eval() scripts by default</playwright-it>
        [SkipBrowserAndPlatformFact(skipFirefox: true, skipWebkit: true)]
        public async Task ShouldIgnoreEvalScriptsByDefault()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Single(coverage);
        }

        ///<playwright-file>chromium/chromium.spec.js</playwright-file>
        ///<playwright-describe>JSCoverage</playwright-describe>
        ///<playwright-it>shouldn't ignore eval() scripts if reportAnonymousScripts is true</playwright-it>
        [SkipBrowserAndPlatformFact(skipFirefox: true, skipWebkit: true)]
        public async Task ShouldNotIgnoreEvalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await Page.Coverage.StartJSCoverageAsync(reportAnonymousScripts: true);
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/eval.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Equal("console.log(\"foo\")", coverage.FirstOrDefault(entry => entry.Url == string.Empty).Source);
            Assert.Equal(2, coverage.Length);
        }

        ///<playwright-file>chromium/chromium.spec.js</playwright-file>
        ///<playwright-describe>JSCoverage</playwright-describe>
        ///<playwright-it>should ignore playwright internal scripts if reportAnonymousScripts is true</playwright-it>
        [SkipBrowserAndPlatformFact(skipFirefox: true, skipWebkit: true)]
        public async Task ShouldIgnorePlaywrightInternalScriptsIfReportAnonymousScriptsIsTrue()
        {
            await Page.Coverage.StartJSCoverageAsync(reportAnonymousScripts: true);
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateAsync("console.log('foo')");
            await Page.EvaluateAsync("() => console.log('bar')");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Empty(coverage);
        }

        ///<playwright-file>chromium/chromium.spec.js</playwright-file>
        ///<playwright-describe>JSCoverage</playwright-describe>
        ///<playwright-it>should report multiple scripts</playwright-it>
        [SkipBrowserAndPlatformFact(skipFirefox: true, skipWebkit: true)]
        public async Task ShouldReportMultipleScripts()
        {
            await Page.Coverage.StartJSCoverageAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/jscoverage/multiple.html");
            var coverage = await Page.Coverage.StopJSCoverageAsync();
            Assert.Equal(2, coverage.Length);
            var orderedList = coverage.OrderBy(c => c.Url).ToArray();
            Assert.Contains("/jscoverage/script1.js", orderedList[0].Url);
            Assert.Contains("/jscoverage/script2.js", orderedList[1].Url);
        }
    }
}
