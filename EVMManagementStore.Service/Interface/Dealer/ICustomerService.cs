using EVMManagementStore.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Interface.Dealer
{
    public interface ICustomerService
    {
       Task<List<CustomerDTO>> GetAllCustomersAsync();
       Task<CustomerDTO> GetCustomersByIdAsync(int customerid);
       Task<CustomerDTO> CreateCustomersAsync(CustomerDTO customerDto);
       Task<CustomerDTO> UpdateCustomersAsync(int customerId, CustomerDTO customerDto);
       Task<bool> DeleteCustomersAsync(int customerid);
    }
}
