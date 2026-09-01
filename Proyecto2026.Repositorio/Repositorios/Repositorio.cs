using Proyecto2026.BD.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Proyecto2026.Repositorio.Repositorios;

public class Repositorio<E> : IRepositorio<E> where E : class
{
    private readonly AppDbContext context;

    public Repositorio(AppDbContext context)
    {
        this.context = context;
    }

    //select
    public async Task<List<E>> Select()
    {
        return await context.Set<E>().ToListAsync();
    }

    //existe
    public async Task<bool> Existe(int id)
    {
        var entity = await context.Set<E>().FindAsync(id);
        return entity != null;
    }

    //insert
    public async Task<E> Insert(E entidad)
    {
        context.Set<E>().Add(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    //update
    public async Task<E> Update(E entidad)
    {
        context.Set<E>().Update(entidad);
        await context.SaveChangesAsync();
        return entidad;
    }

    //delete
    public async Task<bool> Delete(int id)
    {
        var entity = await context.Set<E>().FindAsync(id);
        if (entity == null)
        {
            return false;
        }
        context.Set<E>().Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }





    //programar metodos
    //existe, select, insert, update, delete 
}
