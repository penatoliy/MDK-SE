namespace MDK.Modularity
{
    public abstract class Module : Disposable, IModule
    {
        public IMDK MDK { get; private set; }

        public void Begin(IMDK mdk)
        {
            MDK = mdk;
            OnInitialize();
        }

        protected virtual void OnInitialize() { }
    }
}