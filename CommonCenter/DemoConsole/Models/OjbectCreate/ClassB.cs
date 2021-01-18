using System;
using System.Collections.Generic;
using System.Text;

namespace DemoConsole.Models.OjbectCreate
{
    public class ClassB
    {
        private string _name;

        private int _age;

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public ClassB(string name)
        {
            this._name = name;
        }

        public ClassB(string name, int age)
        {
            this._name = name;
            this._age = age;
        }
    }
}
