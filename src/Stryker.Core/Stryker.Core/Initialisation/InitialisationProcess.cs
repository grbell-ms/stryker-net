using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    // For mocking purposes
    public interface IInitialisationProcessProvider
    {
        IInitialisationProcess Provide();
    }

    [ExcludeFromCodeCoverage]
    public class InitialisationProcessProvider : IInitialisationProcessProvider
    {
        public IInitialisationProcess Provide() => new InitialisationProcess();
    }

    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(IStrykerOptions options);
        InitialTestRun InitialTest(IStrykerOptions options);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private ITestRunner _testRunner;
        private readonly ILogger _logger;

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            ITestRunner testRunner = null,
            IAssemblyReferenceResolver assemblyReferenceResolver = null)
        {
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
        }

        public MutationTestInput Initialize(IStrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options);

            // initial build
            var testProjects = projectInfo.TestProjectAnalyzerResults.ToList();
            for (var i = 0; i < testProjects.Count; i++)
            {
                _logger.LogInformation(
                    "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                    testProjects[i].ProjectFilePath, i + 1,
                    projectInfo.TestProjectAnalyzerResults.Count());

                _initialBuildProcess.InitialBuild(
                    testProjects[i].GetTargetFramework() == Framework.DotNetClassic,
                    testProjects[i].ProjectFilePath,
                    options.SolutionPath);
            }

            if (_testRunner == null)
            {
                _testRunner = new TestRunnerFactory().Create(options, projectInfo);
            }

            var input = new MutationTestInput
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner,
            };

            return input;
        }

        public InitialTestRun InitialTest(IStrykerOptions options) =>
            // initial test
            _initialTestProcess.InitialTest(options, _testRunner);
    }
}
