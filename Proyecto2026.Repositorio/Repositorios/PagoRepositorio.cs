using Proyecto2026.BD.Datos;
using Proyecto2026.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Proyecto2026.Repositorio.Repositorios;

public class PagoRepositorio : Repositorio<Pago>, IPagoRepositorio1
{
    private readonly AppDbContext context;
    public PagoRepositorio(AppDbContext context) : base(context)
    {
        this.context = context;
    }

    public async Task<List<Cliente>> ClientesSaldoPendiente()
    {
        return await context.Clientes
                 .Where(c => c.SaldoPendiente > 0)
                 .OrderByDescending(c => c.SaldoPendiente)
                 .ToListAsync();
    }

    public async Task<Cliente> ObtenerClienteId(int clienteId)
    {
        return await context.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId);
    }

    public async Task<Pago> RegistrarPago(Pago pago, Cliente cliente)
    {
        using var transaccion = await context.Database.BeginTransactionAsync();


        try
        {
            context.Pagos.Add(pago); //agregar el pago

            cliente.SaldoPendiente -= pago.MontoAbonado;
            if (cliente.SaldoPendiente < 0)
                cliente.SaldoPendiente = 0;
            //Resta el monto pagado al saldo del cliente. El if es un resguardo para que nunca quede negativo.

            context.Clientes.Update(cliente); //Marca al cliente (con el saldo ya modificado) para que también se actualice en la base.

            await context.SaveChangesAsync();
            await transaccion.CommitAsync();
            //preparás el pago nuevo y el saldo actualizado en memoria, y con SaveChangesAsync() los grabás juntos en la base de datos.

            return pago;
        }
        catch
        {
            await transaccion.RollbackAsync(); //deshace todo lo que se intentó guardar en esta transacción (el pago y el cambio de saldo)
                                               //como si nunca hubiera pasado.
            throw;
        }



    }
}
