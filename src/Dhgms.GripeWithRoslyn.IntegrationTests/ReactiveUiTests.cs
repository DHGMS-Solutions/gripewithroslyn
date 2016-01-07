namespace Dhgms.GripeWithRoslyn.IntegrationTests
{
    using ReactiveUI;

    public class ReactiveUiTests
    {
        public class ClassWithoutReactiveObjectViewModel
        {
        }

        public class ClassWithReactiveObjectViewModel : ReactiveObject
        {
        }
    }
}