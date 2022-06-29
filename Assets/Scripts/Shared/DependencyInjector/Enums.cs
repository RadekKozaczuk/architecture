namespace Shared.DependencyInjector
{
    public enum ScopeTypes
    {
        Transient,
        Singleton
    }

    public enum ToChoices
    {
        Self,
        Concrete
    }

    public enum BindingInheritanceMethods
    {
        None,
        CopyIntoAll,
        CopyDirectOnly,
        MoveIntoAll,
        MoveDirectOnly
    }

    public enum InjectSources
    {
        Any,
        Local,
        Parent,
        AnyParent
    }
}