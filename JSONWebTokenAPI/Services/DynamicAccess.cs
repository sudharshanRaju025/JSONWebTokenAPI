using DynamicAuthorization.Mvc.Core;
using DynamicAuthorization.Mvc.Core.Models;
using DynamicAuthorization.Mvc.MsSqlServerStore;
using Microsoft.AspNetCore.Identity;
using JSONWebTokenAPI.Model;


namespace JSONWebTokenAPI.Services
{
    public class DynamicAccess
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRoleAccessStore roleAccessStore;
        private readonly IMvcControllerDiscovery mvcControllerDiscovery;

        public DynamicAccess(
            RoleManager<IdentityRole> _roleManager,
            IRoleAccessStore _roleAccessStore, 
            IMvcControllerDiscovery _mvcControllerDiscovery)
        {
            roleManager = _roleManager;
            roleAccessStore = _roleAccessStore;
            mvcControllerDiscovery = _mvcControllerDiscovery;
        }

        public async Task<bool> CreateRoleWithAccessAsync(string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole(roleName);
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded) return false;

                // Discover controllers/actions dynamically
                var controllers = mvcControllerDiscovery.GetControllers();

                // Assign all actions on all controllers to this role (you can filter as needed)
                var roleAccess = new RoleAccess
                {
                    RoleId = role.Id,
                    Controllers = (IEnumerable<MvcControllerInfo>)controllers.Select(c => new ControllerAccess
                    {
                        Controller = c.Name,
                        Actions = c.Actions.Select(a => new ActionAccess { Action = a.Name }).ToList()
                    }).ToList()
                };

                await roleAccessStore.AddRoleAccessAsync(roleAccess);
                return true;
            }
            return false;
        }
    }
    }

