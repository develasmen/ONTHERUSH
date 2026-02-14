using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ONTHERUSH.Abstracciones.DTOs;
using ONTHERUSH.Abstracciones.Interfaces;
using ONTHERUSH.AccesoADatos.Data;
using ONTHERUSH.AccesoADatos.Models;

namespace ONTHERUSH.AccesoADatos.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<object>> ObtenerTodosLosUsuarios()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            return usuarios.Cast<object>().ToList();
        }

        public async Task<ResultadoOperacion> AsignarRolAUsuario(object usuario, string rol)
        {
            var user = (ApplicationUser)usuario;
            var result = await _userManager.AddToRoleAsync(user, rol);

            if (result.Succeeded)
            {
                return new ResultadoOperacion
                {
                    Exito = true,
                    Mensaje = $"Rol '{rol}' asignado exitosamente"
                };
            }

            return new ResultadoOperacion
            {
                Exito = false,
                Mensaje = "Error al asignar el rol"
            };
        }

        public async Task<bool> RolExiste(string rol)
        {
            return await _roleManager.RoleExistsAsync(rol);
        }

        public async Task<ResultadoOperacion> CrearAdministrador(string userId)
        {
            var administrador = new Administrador
            {
                UserId = userId,
                Cargo = "Administrador General",
                FechaContratacion = DateTime.Now
            };

            _context.Administradores.Add(administrador);
            await _context.SaveChangesAsync();

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = "Registro de administrador creado"
            };
        }

        public async Task<ResultadoOperacion> CrearConductor(string userId)
        {
            var conductor = new Conductor
            {
                UserId = userId,
                Estado = true,
                FechaContratacion = DateTime.Now
            };

            _context.Conductores.Add(conductor);
            await _context.SaveChangesAsync();

            return new ResultadoOperacion
            {
                Exito = true,
                Mensaje = "Registro de conductor creado"
            };
        }
    }
}