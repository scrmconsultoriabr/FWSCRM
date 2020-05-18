using System;
namespace FWSCRM.BD
{
    public class KeyAttribute : Attribute
    {
        // Fields
        private ColumnType _cType;
        private bool _inc;
        private string _keyName;

        // Methods
        /// <summary>
        /// Apenas nome da chave - Assume tipo int32
        /// </summary>
        /// <param name="key"></param>
        public KeyAttribute(string key)
        {
            this._keyName = key;
            this._inc = false;
            this.CType = ColumnType.Int32;
        }

        /// <summary>
        /// Nome da Chave e o Tipo
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public KeyAttribute(string key, ColumnType type)
        {
            this._keyName = key;
            this._inc = false;
            this.CType = type;
        }

        /// <summary>
        /// Nome da chave e valor boolean informando sobre auto incremente, assume tipo int32
        /// </summary>
        /// <param name="key"></param>
        /// <param name="inc"></param>
        public KeyAttribute(string key, bool inc)
        {
            this._keyName = key;
            this._inc = inc;
            this.CType = ColumnType.Int32;
        }

        /// <summary>
        /// Col
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="inc"></param>
        public KeyAttribute(string key, ColumnType type, bool inc)
        {
            this._keyName = key;
            this._inc = inc;
            this.CType = ColumnType.Int32;
        }

        // Properties
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

        public string KeyName
        {
            get
            {
                return this._keyName;
            }
            set
            {
                this._keyName = value;
            }
        }
    }

}