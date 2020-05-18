using System;
using System.Data.Common;
using System.Configuration;

namespace FWSCRM.BD
{
    public class Transacao:PersistClass
    {
        // Fields
        public DbConnection con = null;
        private ConnectionStringSettings conSettings = null;
        private DbProviderFactory factory = null;
        public DbTransaction trans = null;

        // Methods
        public void CommitTransaction()
        {
            try
            {
                this.trans.Commit();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao comitar transa\x00e7\x00e3o: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
                this.trans = null;
            }
        }

        private string getProviderName()
        {
            string key = this.getAppSettings();
            this.conSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings[key]];
            return this.conSettings.ProviderName;
        }

        public void RollbackTransaction()
        {
            try
            {
                if (this.trans != null)
                {
                    this.trans.Rollback();
                }
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro no rollback da transa\x00e7\x00e3o: " + dbE.Message);
            }
            finally
            {
                this.con.Close();
                this.trans = null;
            }
        }

        public void StartTransaction()
        {
            try
            {
                this.factory = DbProviderFactories.GetFactory(this.getProviderName());
                this.con = this.factory.CreateConnection();
                this.con.ConnectionString = base.getConnectionString();
                this.con.Open();
                this.trans = this.con.BeginTransaction();
            }
            catch (DbException dbE)
            {
                throw new ArgumentException("Erro ao criar DbFactory: " + dbE.Message);
            }
        }
    }

}