using System;

namespace FWSCRM.BD
{
    public class ColumnAttribute : Attribute
    {
        // Fields
        private string _columnName;
        private int _columnSize;
        private ColumnType _cType;
        private bool _null;
        private bool _inc;
        private bool _temp;
        // Methods
        public ColumnAttribute(string columnName, ColumnType type)
        {
            this._columnName = columnName;
            this._cType = type;
            this._columnSize = 0;
            this._null = true;
            this._inc = false;
            this._temp = false;
        }

        public ColumnAttribute(string columnName, ColumnType type, bool Inc )
        {
            this._columnName = columnName;
            this._cType = type;
            this._columnSize = 0;
            this._null = true;
            this._inc = Inc;
            this._temp = false;
        }

        public ColumnAttribute(string columnName, ColumnType type, int size)
        {
            this._columnName = columnName;
            this._cType = type;
            this._columnSize = size;
            this._null = true;
            this._inc = false;
            this._temp = false;
        }

        public ColumnAttribute(string columnName, ColumnType type, int size, bool nulo)
        {
            this._columnName = columnName;
            this._cType = type;
            this._columnSize = size;
            this._null = nulo;
            this._inc = false;
            this._temp = false;
        }

        public ColumnAttribute(string columnName, ColumnType type, int size, bool nulo, bool temp)
        {
            this._columnName = columnName;
            this._cType = type;
            this._columnSize = size;
            this._null = nulo;
            this._inc = false;
            this._temp = true;
        }

        // Properties
        public string ColumnName
        {
            get
            {
                return this._columnName;
            }
        }

        public int ColumnSize
        {
            get
            {
                return this._columnSize;
            }
            set
            {
                this._columnSize = value;
            }
        }

        public ColumnType CType
        {
            get
            {
                return this._cType;
            }
            set
            {
                this._cType = value;
            }
        }

        public bool Null
        {
            get
            {
                return this._null;
            }
            set
            {
                this._null = value;
            }
        }

        public bool Inc
        {
            get
            {
                return this._inc;
            }
            set
            {
                this._inc = value;
            }
        }

        public bool Temp
        {
            get
            {
                return this._temp;
            }
            set
            {
                this._temp = value;
            }
        }
    }
}