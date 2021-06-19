using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiendaRamiros.Presentacion
{
    public partial class FormAgregarDisponibilidad : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["TiendaRamiros.Properties.Settings.TiendaDBConnectionString"].ConnectionString;
        public FormAgregarDisponibilidad()
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cbProducto.SelectedIndex == 0 || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Seleccione un Producto/Cantidad");
            }
            else
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("UPDATE Productos SET Disponible = Disponible + @Disponible WHERE Id_Prod = @Id_Prod", connection))
                    {
                        string idProducto = cbProducto.SelectedValue.ToString();
                        command.Parameters.AddWithValue("Disponible", Convert.ToDecimal(txtCantidad.Text));
                        command.Parameters.AddWithValue("Id_Prod", Convert.ToInt32(idProducto));
                        var r = command.ExecuteNonQuery();

                        txtCantidad.Text = string.Empty;
                        
                        cbProducto.SelectedIndex = 0;

                        MessageBox.Show("Cantidad Ingresada con éxito");
                    }
                }
            }
            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (cbProducto.SelectedIndex == 0 || string.IsNullOrEmpty(txtCantidad.Text))
            {
                MessageBox.Show("Seleccione un Producto/Cantidad");
            }
            else
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("UPDATE Productos SET Disponible = Disponible - @Disponible WHERE Id_Prod = @Id_Prod", connection))
                    {
                        string idProducto = cbProducto.SelectedValue.ToString();
                        command.Parameters.AddWithValue("Disponible", Convert.ToDecimal(txtCantidad.Text));
                        command.Parameters.AddWithValue("Id_Prod", Convert.ToInt32(idProducto));
                        var r = command.ExecuteNonQuery();

                        txtCantidad.Text = string.Empty;

                        cbProducto.SelectedIndex = 0;

                        MessageBox.Show("Cantidad restada con éxito");
                    }
                }
            }
        }

        private void FormAgregarDisponibilidad_Load(object sender, EventArgs e)
        {
            populateProducto();
        }

        void populateProducto()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT [Id_Prod],p.[Descripcion] + ' / ' + U.Descripcion as DescripcionUnidad FROM [dbo].[Productos] P inner join TipoUnidad U on P.Id_TipoUnidad = U.Id_TipoUnidad", sqlCon);
                DataTable dtProductos = new DataTable();
                sqlDa.Fill(dtProductos);
                cbProducto.ValueMember = "Id_Prod";
                cbProducto.DisplayMember = "DescripcionUnidad";
                DataRow topItem = dtProductos.NewRow();
                topItem[0] = 0;
                topItem[1] = "Seleccione...";
                dtProductos.Rows.InsertAt(topItem, 0);

                cbProducto.DataSource = dtProductos;

            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {

                e.Handled = true;

            }

        }

        private static void OnlyNumber(KeyPressEventArgs e, bool isdecimal)
        {
            String aceptados;
            if (!isdecimal)
            {
                aceptados = "0123456789," + Convert.ToChar(8);
            }
            else
                aceptados = "0123456789." + Convert.ToChar(8);

            if (aceptados.Contains("" + e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        
    }
}
