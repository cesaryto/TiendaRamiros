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
using TiendaRamiros.Presentacion;
using Excel = Microsoft.Office.Interop.Excel;
namespace TiendaRamiros
{
    public partial class Form1 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["TiendaRamiros.Properties.Settings.TiendaDBConnectionString"].ConnectionString;
        decimal dcmTotal = 0;
        public Form1()
        {
            InitializeComponent();
            tabControl1.SelectedIndex = 3;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            populateProducto();
        }

        #region llenado de combos

        void populateTipoProducto()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM TipoProd", sqlCon);
                DataTable dtTipoProd = new DataTable();
                sqlDa.Fill(dtTipoProd);
                cbTipoProd.ValueMember = "Id_TipoProd";
                cbTipoProd.DisplayMember = "Descripcion";
                //DataRow topItem = dtTipoProd.NewRow();
                //topItem[0] = 0;
                //topItem[1] = "Seleccione...";
                //dtTipoProd.Rows.InsertAt(topItem,0);

                cbTipoProd.DataSource = dtTipoProd;

            }
            

        }

        void populateTipoUnidad()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM TipoUnidad", sqlCon);
                DataTable dtTipoUnidad = new DataTable();
                sqlDa.Fill(dtTipoUnidad);
                cbTipoUnidad.ValueMember = "Id_TipoUnidad";
                cbTipoUnidad.DisplayMember = "Descripcion";
                //DataRow topItem = dtTipoUnidad.NewRow();
                //topItem[0] = 0;
                //topItem[1] = "Seleccione...";
                //dtTipoUnidad.Rows.InsertAt(topItem, 0);

                cbTipoUnidad.DataSource = dtTipoUnidad;

            }
        }

        void populateProducto()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT [Id_Prod],p.[Descripcion] + ' / ' + U.Descripcion as DescripcionUnidad FROM [dbo].[Productos] P inner join TipoUnidad U on P.Id_TipoUnidad = U.Id_TipoUnidad", sqlCon);
                DataTable dtProductos = new DataTable();
                sqlDa.Fill(dtProductos);
                cbProductoCajero.ValueMember = "Id_Prod";
                cbProductoCajero.DisplayMember = "DescripcionUnidad";
                DataRow topItem = dtProductos.NewRow();
                topItem[0] = 0;
                topItem[1] = "Seleccione...";
                dtProductos.Rows.InsertAt(topItem, 0);

                cbProductoCajero.DataSource = dtProductos;

            }
        }

        #endregion

        #region llenado de DataGridViews

        void populateDatagridTipoProducto()
        {
            

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM TipoProd",sqlCon);
                DataTable dtTipoProd = new DataTable();
                sqlDa.Fill(dtTipoProd);
                dgTiposProducto.DataSource = dtTipoProd;

            }
        }

        void populateDatagridTipoUnidad()
        {
           

            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM TipoUnidad", sqlCon);
                DataTable dtTipoUnidad = new DataTable();
                sqlDa.Fill(dtTipoUnidad);
                dgTipoUnidad.DataSource = dtTipoUnidad;

            }
        }

        void populateDatagridProductos()
        {


            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM Productos", sqlCon);
                DataTable dtProductos = new DataTable();
                sqlDa.Fill(dtProductos);
                dgProductos.DataSource = dtProductos;

            }
        }

        void populateDatagridVentas()
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {

                SqlCommand command = new SqlCommand("spVentasGetAll", sqlCon);
                command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dt = new DataTable();

                sqlCon.Open();

                adapter.Fill(dt);

                dgVentasTodas.DataSource = dt;


            }
        }

        #endregion

        private void dgTiposProducto_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgTiposProducto.CurrentRow != null)
            {
               

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    DataGridViewRow dgvRow = dgTiposProducto.CurrentRow;
                    SqlCommand sqlCmd = new SqlCommand("spTipoproductoAddorEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    if (dgvRow.Cells["txtTipoProductoId"].Value == DBNull.Value)//Insert
                        sqlCmd.Parameters.AddWithValue("@IdTipoProducto", 0);
                    else//update
                        sqlCmd.Parameters.AddWithValue("@IdTipoProducto", Convert.ToInt32(dgvRow.Cells["txtTipoProductoId"].Value));

                    sqlCmd.Parameters.AddWithValue("@Descripcion", dgvRow.Cells["txtTipoProductoDescripcion"].Value==DBNull.Value ? " " : dgvRow.Cells["txtTipoProductoDescripcion"].Value.ToString());

                    string dr = sqlCmd.ExecuteScalar().ToString();
                }

                populateDatagridProductos();

                populateProducto();
            }
        }

        private void dgTiposProducto_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if (dgTiposProducto.CurrentRow.Cells["txtTipoProductoId"].Value != DBNull.Value)
                {
                    if (MessageBox.Show("Esta seguro de eliminar este tipo de producto ?", "DataGridView", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        using (SqlConnection sqlCon = new SqlConnection(connectionString))
                        {
                            sqlCon.Open();
                            DataGridViewRow dgvRow = dgTiposProducto.CurrentRow;
                            SqlCommand sqlCmd = new SqlCommand("spTipoProdDeletebyId", sqlCon);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@IdTipoProd", Convert.ToInt32(dgvRow.Cells["txtTipoProductoId"].Value));
                            sqlCmd.ExecuteNonQuery();

                        }
                    }
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se puede eliminar el registro por referencias.");
                e.Cancel = true;
            }

        }

        private void dgTipoUnidad_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgTipoUnidad.CurrentRow != null)
            {
               

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    sqlCon.Open();
                    DataGridViewRow dgvRow = dgTipoUnidad.CurrentRow;
                    SqlCommand sqlCmd = new SqlCommand("spTipoUnidadAddorEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    if (dgvRow.Cells["txtTipoUnidadId"].Value == DBNull.Value)//Insert
                        sqlCmd.Parameters.AddWithValue("@IdTipoUnidad", 0);
                    else//update
                        sqlCmd.Parameters.AddWithValue("@IdTipoUnidad", Convert.ToInt32(dgvRow.Cells["txtTipoUnidadId"].Value));

                    sqlCmd.Parameters.AddWithValue("@Descripcion", dgvRow.Cells["txtTipoUnidadDescripcion"].Value == DBNull.Value ? " " : dgvRow.Cells["txtTipoUnidadDescripcion"].Value.ToString());

                    sqlCmd.ExecuteNonQuery();

                }

            }
        }

        private void dgTipoUnidad_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if (dgTipoUnidad.CurrentRow.Cells["txtTipoUnidadId"].Value != DBNull.Value)
                {
                    if (MessageBox.Show("Esta seguro de eliminar este tipo de unidad ?", "DataGridView", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        using (SqlConnection sqlCon = new SqlConnection(connectionString))
                        {
                            sqlCon.Open();
                            DataGridViewRow dgvRow = dgTipoUnidad.CurrentRow;
                            SqlCommand sqlCmd = new SqlCommand("spTipoUnidadDeletebyId", sqlCon);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@IdTipoUnidad", Convert.ToInt32(dgvRow.Cells["txtTipoUnidadId"].Value));
                            sqlCmd.ExecuteNonQuery();

                        }
                    }
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("No se puede eliminar el registro por referencias.");
                e.Cancel = true;
            }
            

        }

        private void dgProductos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgProductos.CurrentRow != null)
            {
                try
                {
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        sqlCon.Open();
                        DataGridViewRow dgvRow = dgProductos.CurrentRow;

                        if (!string.IsNullOrEmpty(dgvRow.Cells["cbTipoProd"].Value.ToString()) && !string.IsNullOrEmpty(dgvRow.Cells["cbTipoUnidad"].Value.ToString()) && !string.IsNullOrEmpty(dgvRow.Cells["txtDisponible"].Value.ToString()) && !string.IsNullOrEmpty(dgvRow.Cells["txtValorUnidad"].Value.ToString()) && !string.IsNullOrEmpty(dgvRow.Cells["PrecioCompraUnidad"].Value.ToString()))
                        {
                            SqlCommand sqlCmd = new SqlCommand("spProductoAddorEdit", sqlCon);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            if (dgvRow.Cells["txtproductId"].Value == DBNull.Value)//Insert
                                sqlCmd.Parameters.AddWithValue("@Id_Prod", 0);
                            else//update
                                sqlCmd.Parameters.AddWithValue("@Id_Prod", Convert.ToInt32(dgvRow.Cells["txtproductId"].Value));

                            sqlCmd.Parameters.AddWithValue("@Descripcion", dgvRow.Cells["txtDescripcion"].Value == null ? " " : dgvRow.Cells["txtDescripcion"].Value.ToString().TrimEnd());
                            sqlCmd.Parameters.AddWithValue("@Id_TipoProd", Convert.ToInt32(dgvRow.Cells["cbTipoProd"].Value == null ? "0" : dgvRow.Cells["cbTipoProd"].Value.ToString()));
                            sqlCmd.Parameters.AddWithValue("@Id_TipoUnidad", Convert.ToInt32(dgvRow.Cells["cbTipoUnidad"].Value == null ? "0" : dgvRow.Cells["cbTipoUnidad"].Value.ToString()));
                            sqlCmd.Parameters.AddWithValue("@Disponible", Convert.ToDecimal(dgvRow.Cells["txtDisponible"].Value == null ? "0" : dgvRow.Cells["txtDisponible"].Value.ToString()));
                            sqlCmd.Parameters.AddWithValue("@Valor_Unidad", Convert.ToDecimal(dgvRow.Cells["txtValorUnidad"].Value == null ? "0" : dgvRow.Cells["txtValorUnidad"].Value.ToString()));
                            sqlCmd.Parameters.AddWithValue("@PrecioCompraUnidad", Convert.ToDecimal(dgvRow.Cells["PrecioCompraUnidad"].Value == null ? "0" : dgvRow.Cells["PrecioCompraUnidad"].Value.ToString()));
                            sqlCmd.ExecuteNonQuery();
                            populateDatagridProductos();

                        }

                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("REVISE LOS DATOS ["+ ex.Message+"]");
                }

            }
        }

        private void dgProductos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgProductos.CurrentCell.ColumnIndex == 6 || dgProductos.CurrentCell.ColumnIndex == 4 || dgProductos.CurrentCell.ColumnIndex == 5)
            {
                e.Control.KeyPress -= AllowNumbersOnly;
                e.Control.KeyPress += AllowNumbersOnly;
            }
        }

        private void AllowNumbersOnly(Object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',')
                e.Handled = true;
            if (e.KeyChar == ',' && (sender as TextBox).Text.IndexOf(',') > -1)
            {
                e.Handled = true;
            }
        }

        private void btnAgregarDIsponibilidad_Click(object sender, EventArgs e)
        {
            using (FormAgregarDisponibilidad popUpForm = new FormAgregarDisponibilidad())
            {
                popUpForm.FormClosed += new System.Windows.Forms.FormClosedEventHandler(formAgregarDisponibilidad_FormClosed);
                popUpForm.ShowDialog();
            }
        }

        private void formAgregarDisponibilidad_FormClosed(object sender, FormClosedEventArgs e)
        {
            populateDatagridProductos();
        }

        private void txtCantidadCajero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {

                e.Handled = true;

            }
        }

        private void btnAgregarProductoCaja_Click(object sender, EventArgs e)
        {
            if (cbProductoCajero.SelectedIndex == 0 || string.IsNullOrEmpty(txtCantidadCajero.Text))
            {
                MessageBox.Show("Ingrese producto/cantidad");
            }
            else
            {
                string idProducto = cbProductoCajero.SelectedValue.ToString();
                string r = string.Empty;



                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT [Valor_Unidad] FROM [dbo].[Productos] WHERE Id_Prod=@Id_Prod", connection))
                    {
                       
                        command.Parameters.AddWithValue("Id_Prod", Convert.ToInt32(idProducto));
                        r = command.ExecuteScalar().ToString();

                        
                    }
                }


                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgVentaCajero);
                row.Cells[0].Value = cbProductoCajero.Text;
                row.Cells[1].Value = r.ToString();
                row.Cells[2].Value = txtCantidadCajero.Text;
                row.Cells[3].Value = (Convert.ToDecimal(txtCantidadCajero.Text)*Convert.ToDecimal(r)).ToString();
                row.Cells[4].Value = idProducto;
                dgVentaCajero.Rows.Add(row);

                dcmTotal = dcmTotal + Convert.ToDecimal(txtCantidadCajero.Text) * Convert.ToDecimal(r);

                lblTotalValor.Text = dcmTotal.ToString("$ 0");

                txtCantidadCajero.Text = string.Empty;

                cbProductoCajero.SelectedIndex = 0;
            }
         
        }

        private void dgVentaCajero_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
           string strValor = dgVentaCajero.CurrentRow.Cells[2].Value.ToString();
           dcmTotal = dcmTotal - Convert.ToDecimal(strValor);
           lblTotalValor.Text = dcmTotal.ToString();
        }

        private void btnRegistrarVenta_Click(object sender, EventArgs e)
        {
            string r = string.Empty;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO [dbo].[Ventas] ([Fecha_Hora],[Valor]) VALUES (@Fecha_Hora, @Valor); SELECT @@IDENTITY", connection))
                {
                    
                    command.Parameters.AddWithValue("Fecha_Hora", DateTime.Now);
                    command.Parameters.AddWithValue("Valor", Convert.ToDecimal(lblTotalValor.Text.Substring(1)));
                    r = command.ExecuteScalar().ToString();

                    foreach (DataGridViewRow dgvr in dgVentaCajero.Rows)
                    {
                        using (var cmd = new SqlCommand("INSERT INTO [dbo].[VentaProducto] ([Id_Venta],[Id_Producto],[Unidades],[Valor]) VALUES (@Id_Venta, @Id_Producto, @Unidades,@Valor); UPDATE Productos SET Disponible = Disponible - @Unidades WHERE Id_Prod= @Id_Producto", connection))
                        {
                            cmd.Parameters.AddWithValue("Id_Venta", Convert.ToInt32(r));
                            cmd.Parameters.AddWithValue("Id_Producto", Convert.ToInt32(dgvr.Cells[4].Value.ToString()));
                            cmd.Parameters.AddWithValue("Unidades", Convert.ToDecimal(dgvr.Cells[2].Value.ToString()));
                            cmd.Parameters.AddWithValue("Valor", Convert.ToDecimal(dgvr.Cells[3].Value.ToString()));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            dgVentaCajero.Rows.Clear();
            dcmTotal = 0;
            lblTotalValor.Text = string.Empty;
            MessageBox.Show("Venta registrada exitosamente");

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                populateDatagridTipoProducto();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                populateDatagridTipoUnidad();
            }
            else if (tabControl1.SelectedIndex == 2)
            {
                populateTipoProducto();
                populateTipoUnidad();

                populateDatagridProductos();
            }
            else if (tabControl1.SelectedIndex == 3)
            {
                populateProducto();
            }
            else if (tabControl1.SelectedIndex == 4)
            {
                populateDatagridVentas();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            int i = 0;
            int j = 0;

            //for (i = 0; i <= dgVentasTodas.RowCount - 1; i++)
            //{
            //    for (j = 0; j <= dgVentasTodas.ColumnCount - 1; j++)
            //    {
            //        DataGridViewCell cell = dgVentasTodas[j, i];
            //        xlWorkSheet.Cells[i + 1, j + 1] = cell.Value;
            //    }
            //}

            xlWorkSheet.Cells[1, 1] = "ID";
            xlWorkSheet.Cells[1, 2] = "FechaHora";
            xlWorkSheet.Cells[1, 3] = "Producto";
            xlWorkSheet.Cells[1, 4] = "TipoUnidad";
            xlWorkSheet.Cells[1, 5] = "PrecioCompraUnidad";
            xlWorkSheet.Cells[1, 6] = "Cantidad";
            xlWorkSheet.Cells[1, 7] = "ValorProductos";
            xlWorkSheet.Cells[1, 8] = "TotalVenta";


            for (i = 1; i <= dgVentasTodas.RowCount; i++)
            {
                for (j = 0; j <= dgVentasTodas.ColumnCount - 1; j++)
                {
                    DataGridViewCell cell = dgVentasTodas[j, i-1];
                    xlWorkSheet.Cells[i+1, j + 1] = cell.Value;
                }
            }

            string dt=DateTime.Now.ToString().Trim().Replace("/","").Replace(":","").Replace(".","");
            xlWorkBook.SaveAs(dt+"Ventas.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            

            MessageBox.Show("Excel file created");
        }

        private void dgProductos_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                if (dgProductos.CurrentRow.Cells["txtproductId"].Value != DBNull.Value)
                {
                    if (MessageBox.Show("Esta seguro de eliminar este producto ?", "DataGridView", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        using (SqlConnection sqlCon = new SqlConnection(connectionString))
                        {
                            sqlCon.Open();
                            DataGridViewRow dgvRow = dgProductos.CurrentRow;
                            SqlCommand sqlCmd = new SqlCommand("spProductoDeletebyId", sqlCon);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@IdProducto", Convert.ToInt32(dgvRow.Cells["txtproductId"].Value));
                            sqlCmd.ExecuteNonQuery();

                        }
                    }
                    else
                        e.Cancel = true;
                }
                else
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se puede eliminar el registro por referencias.");
                e.Cancel = true;
            }
            
        }
    }
}
