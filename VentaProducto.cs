//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TiendaRamiros
{
    using System;
    using System.Collections.Generic;
    
    public partial class VentaProducto
    {
        public int Id_VentaProducto { get; set; }
        public int Id_Venta { get; set; }
        public int Id_Producto { get; set; }
        public decimal Unidades { get; set; }
    
        public virtual Productos Productos { get; set; }
        public virtual Ventas Ventas { get; set; }
    }
}
