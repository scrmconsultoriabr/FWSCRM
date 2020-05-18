using System;

namespace FWSCRM.BD
{
    public class TableAttribute : Attribute
    {
        // Fields
        private string _tableName;

        // Methods
        public TableAttribute(string tableName)
        {
            this._tableName = tableName;
        }

        public override string ToString()
        {
            return this._tableName;
        }

        // Properties
        public string TableName
        {
            get
            {
                return this._tableName;
            }
        }
    }
}