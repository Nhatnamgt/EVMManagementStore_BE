using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.Dealer
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.UserRepository.GetAllAsync();
            return customers
                .Where(u => u.RoleId == 4)
                .Select(u => MapToDTO(u))
                .ToList();
        }

        public async Task<CustomerDTO> GetCustomersByIdAsync(int customerId)
        {
            var customer = await _unitOfWork.UserRepository.GetByIdAsync(customerId);
            return (customer != null && customer.RoleId == 4)
                ? MapToDTO(customer)
                : null;
        }

        public async Task<CustomerDTO> CreateCustomersAsync(CustomerDTO customerDto)
        {
            var entity = new User
            {
                Username = customerDto.Username,
                Email = customerDto.Email,
                PasswordHash = customerDto.PasswordHash,
                RoleId = 4,
                FullName = customerDto.FullName,
                Phone = customerDto.Phone,
                Address = customerDto.Address,
                CompanyName = customerDto.CompanyName
            };

            await _unitOfWork.UserRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();
            customerDto.UserId = entity.UserId;

            return MapToDTO(entity);
        }

        public async Task<CustomerDTO> UpdateCustomersAsync(int customerId, CustomerDTO customerDto)
        {
            var customer = await _unitOfWork.UserRepository.GetByIdAsync(customerId);
            if (customer == null) return null;

            customer.Username = customerDto.Username;
            customer.Email = customerDto.Email;
            customer.PasswordHash = customerDto.PasswordHash;
            customer.FullName = customerDto.FullName;
            customer.Phone = customerDto.Phone;
            customer.Address = customerDto.Address;
            customer.CompanyName = customerDto.CompanyName;

            await _unitOfWork.UserRepository.UpdateAsync(customer);
            await _unitOfWork.SaveAsync();

            return MapToDTO(customer);
        }

        public async Task<bool> DeleteCustomersAsync(int customerId)
        {
            var customer = await _unitOfWork.UserRepository.GetByIdAsync(customerId);
            if (customer == null) return false;

            await _unitOfWork.UserRepository.RemoveAsync(customer);
            await _unitOfWork.SaveAsync();

            return true;
        }
        private static CustomerDTO MapToDTO(User u)
        {
            return new CustomerDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                RoleId = u.RoleId,
                FullName = u.FullName,
                Phone = u.Phone,
                Address = u.Address,
                CompanyName = u.CompanyName
            };
        }
    }
}
