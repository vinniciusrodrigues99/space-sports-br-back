using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSP.Api.Domain.Constants;
using FSP.Api.Domain.Entities.Permission;
using FSP.Api.Domain.Entities.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FSP.Api.Infrastructure.Data.DbContexts
{
    public class Seed
    {
        public static async Task SeedPermissionsAndRolesAsync(ApplicationDbContext context)
        {
            if (context.Permissions.Any())
                return;
            
            var permissions = new List<Permissao>
            {
                new() { Nome = Permissions.AttachmentsRead, NomeExibicao = "Anexos (L)" , Descricao = "Permissão para visualizar anexos", Categoria = PermissionCategories.Attachments, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                new() { Nome = Permissions.AttachmentsWrite, NomeExibicao = "Anexos (E/L)", Descricao = "Permissão para gerenciar anexos", Categoria = PermissionCategories.Attachments, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
        
                new () { Nome = Permissions.CompaniesWrite, NomeExibicao = "Empresa (E/L)", Descricao = "Permissão para gerenciar empresa", Categoria = PermissionCategories.Enterprise, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                
                new () { Nome = Permissions.LogsRead, NomeExibicao = "Logs (L)", Descricao = "Permissão para visualizar logs", Categoria = PermissionCategories.Logs, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                
                new () { Nome = Permissions.PermissionsWrite, NomeExibicao = "Permissões (E/L)", Descricao = "Permissão para gerenciar permissões", Categoria = PermissionCategories.Permissions, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                
                new () { Nome = Permissions.TitlesRead, NomeExibicao = "Títulos (L)", Descricao = "Permissão para visualizar títulos", Categoria = PermissionCategories.Titles, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                new () { Nome = Permissions.TitlesValidation, NomeExibicao = "Títulos (V)", Descricao = "Permissão para validar títulos", Categoria = PermissionCategories.Titles, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                new () { Nome = Permissions.TitlesApproval, NomeExibicao = "Títulos (A)", Descricao = "Permissão para aprovar títulos", Categoria = PermissionCategories.Titles, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },

                new () { Nome = Permissions.ObservationsRead, NomeExibicao = "Observações (L)", Descricao = "Permissão para visualizar observações", Categoria = PermissionCategories.Observations, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
                new () { Nome = Permissions.ObservationsWrite, NomeExibicao = "Observações (E/L)", Descricao = "Permissão para gerenciar observações", Categoria = PermissionCategories.Observations, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },

                new () { Nome = Permissions.UsersWrite, NomeExibicao = "Usuários (E/L)", Descricao = "Permissão para gerenciar usuários", Categoria = PermissionCategories.Users, CriadoEm = DateTime.UtcNow, CriadoPor = "System" },
            };
            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        public static async Task SeedRoleClaimsAsync(ApplicationDbContext context, RoleManager<Perfil> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(ProfileRoles.Admin);
            var diretoriaRole = await roleManager.FindByNameAsync(ProfileRoles.Board);
            var aprovadorRole = await roleManager.FindByNameAsync(ProfileRoles.Approver);

            if (adminRole == null || diretoriaRole == null || aprovadorRole == null)
                return;

            if (await context.RoleClaims.AnyAsync())
                return;

            var roleClaims = new List<IdentityRoleClaim<Guid>>();

            var adminPermissions = new[]
            {
                Permissions.AttachmentsRead,
                Permissions.AttachmentsWrite,
                Permissions.CompaniesWrite,
                Permissions.LogsRead,
                Permissions.PermissionsWrite,
                Permissions.TitlesRead,
                Permissions.TitlesValidation,
                Permissions.TitlesApproval,
                Permissions.ObservationsRead,
                Permissions.ObservationsWrite,
                Permissions.UsersWrite
            };

            foreach (var permission in adminPermissions)
            {
                roleClaims.Add(new IdentityRoleClaim<Guid>
                {
                    RoleId = adminRole.Id,
                    ClaimType = "Permissao",
                    ClaimValue = permission
                });
            }

            var boardPermissions = new[]
            {
                Permissions.TitlesRead,
                Permissions.TitlesValidation,
                Permissions.ObservationsRead,
                Permissions.AttachmentsRead
            };

            foreach (var permission in boardPermissions)
            {
                roleClaims.Add(new IdentityRoleClaim<Guid>
                {
                    RoleId = diretoriaRole.Id,
                    ClaimType = "Permissao",
                    ClaimValue = permission
                });
            }

            var approverPermissions = new[]
            {
                Permissions.TitlesRead,
                Permissions.ObservationsRead,
                Permissions.AttachmentsRead
            };

            foreach (var permission in approverPermissions)
            {
                roleClaims.Add(new IdentityRoleClaim<Guid>
                {
                    RoleId = aprovadorRole.Id,
                    ClaimType = "Permissao",
                    ClaimValue = permission
                });
            }

            await context.RoleClaims.AddRangeAsync(roleClaims);
            await context.SaveChangesAsync();
        }

        public static async Task SeedDefaultUserAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@fsp.com",
                Email = "admin@fsp.com",
                EmailConfirmed = true,
                NomeCompleto = "Administrador do Sistema",
                CPF = "00000000000",
                CriadoPor = "System",
                CriadoEm = DateTimeOffset.Now
            };

            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@12345");
                if (result.Succeeded)   
                {
                    await userManager.AddToRoleAsync(adminUser, ProfileRoles.Admin);
                }
            }
        }

        public static async Task SeedRolesAsync(RoleManager<Perfil> roleManager)
        {
            var roles = new[]
            {
                ProfileRoles.Admin,
                ProfileRoles.Author,
                ProfileRoles.Reader,
                ProfileRoles.Board,
                ProfileRoles.Approver
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new Perfil
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        CriadoPor = "System",
                        CriadoEm = DateTimeOffset.UtcNow
                    };
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}