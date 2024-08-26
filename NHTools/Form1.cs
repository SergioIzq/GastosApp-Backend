using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using AppG.Entidades.BBDD;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace NHTools
{
    public partial class Form1 : Form
    {
        private Configuration cfg;
        private ISessionFactory sessionFactory;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.CompruebaBD();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (cfg == null)
            {
                //log4net.Config.XmlConfigurator.Configure();
                cfg = new Configuration();

                cfg.Configure("hibernate.cfg.xml");

                Assembly thisAssembly = typeof(Categoria).Assembly;
                cfg.AddAssembly(thisAssembly);
            }
        }

        private void CompruebaBD()
        {
            this.Cursor = Cursors.WaitCursor;

            this.richTextBox1.Text = string.Empty;

            try
            {
                SchemaValidator validator = new SchemaValidator(cfg);
                //var v1schemaValidator = new NHibernate.Tool.hbm2ddl.SchemaValidator((cfg));
                //v1schemaValidator.Validate();

                validator.Validate();
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("CORRECTO: La base de datos es correcta!!!");
            }
            catch (MappingException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (HibernateException ex)
            {

                try
                {
                    string scriptTexto = "";
                    Action<string> updateExport = x =>
                    {
                        scriptTexto += x;
                    };

                    listBox1.Items.Clear();
                    SchemaUpdate update = new SchemaUpdate(cfg);
                    update.Execute(updateExport, false);
                    richTextBox1.Text = scriptTexto.Length == 0 ? ex.ToString() : scriptTexto;
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.ToString());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.Cursor = Cursors.Arrow;
        }

        
    }
}
