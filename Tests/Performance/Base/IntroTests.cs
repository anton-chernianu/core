using System.Collections;
using NUnit.Framework;
using ReactUnity.Scripting;
using Unity.PerformanceTesting;

namespace ReactUnity.Tests.Performance
{
    [TestFixture(JavascriptEngineType.Auto)]
    public class IntroTests : TestBase
    {
        public IntroTests(JavascriptEngineType engineType) : base(engineType) { }

        [UGUITest, Performance]
        public IEnumerator TestFramePerformance()
        {
            yield return Measure.Frames().WarmupCount(10).MeasurementCount(60).Run();
        }

        [UGUITest(Script = @"
const arr = new Array(200).fill(0);

function App() {
    return <>
        {arr.map((x,i) => <bg key={i} />)}
    </>;
}
",
            Style = @"
:host {
    flex-direction: row;
    flex-wrap: wrap;
}

bg {
    width: 20px;
    height: 20px;
    background-color: yellow;
}
"), Performance]
        public IEnumerator TestFramePerformanceWithMultipleBackgrounds()
        {
            var batchesCount = new SampleGroup("Draw Calls Count", SampleUnit.Byte);

            yield return Measure.Frames()
                .WarmupCount(10)
                .MeasurementCount(60)
                .ProfilerMarkers(batchesCount)
                .Run();
        }
    }
}
