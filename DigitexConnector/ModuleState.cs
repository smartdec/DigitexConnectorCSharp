namespace DigitexConnector
{
    public class ModuleState
    {
        public string module;
        public string name;
        public string value;

        public ModuleState(string module, string param, string value)
        {
            this.module = module;
            this.name = param;
            this.value = value;
        }

        public ModuleState(string param, string value)
        {
            module = "";
            this.name = param;
            this.value = value;
        }
    }
}
