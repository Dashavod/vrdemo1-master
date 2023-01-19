using _Project.Scripts.Experiment_1.Data;
using DELTation.DIFramework;
using DELTation.DIFramework.Containers;

namespace _Project.Scripts.Experiment_1
{
    public class SceneDependencies : DependencyContainerBase
    {
        protected override void ComposeDependencies(ICanRegisterContainerBuilder builder)
        {
            builder.Register(new CommunicationData
            {
                Value = "No response from WIT"
            });
        }
    }
}