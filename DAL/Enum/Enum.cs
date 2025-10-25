using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Enum
{
    public enum CharacterOrderStatus
        {
            Pending = 0,      // Đang chờ xử lý
            Active = 1,       // Đang hoạt động
            Completed = 2,    // Đã hoàn thành
            Cancelled = 3,    // Đã hủy
        }
    
    public enum UserCharacterStatus
    {
        Inactive = 0,     // Không hoạt động
        Active = 1,       // Đang hoạt động
    }


    public enum CharacterPackageStatus
    {
        Inactive = 0,     // Không hoạt động
        Active = 1,       // Đang hoạt động
        Archived = 2      // Đã lưu trữ
    }




    public enum PaymentStatus
    {
        Pending = 0,      // Đang chờ thanh toán
        Completed = 1,    // Đã thanh toán thành công
        Failed = 2,       // Thanh toán thất bại
        Cancelled = 3,    // Đã hủy
        Refunded = 4      // Đã hoàn tiền
    }


    public enum OrderStatus
    {
        Pending = 0,      // Đang chờ xử lý
        Processing = 1,   // Đang xử lý
        Shipped = 2,      // Đã giao cho vận chuyển
        Delivered = 3,    // Đã giao hàng thành công
        Cancelled = 4,    // Đã hủy
        Returned = 5      // Đã trả hàng
    }

    public enum OrderItemStatus
    {
        Pending = 0,      // Đang chờ xử lý
        Processing = 1,   // Đang xử lý
        Completed = 2,    // Đã hoàn thành
        Cancelled = 3     // Đã hủy
    }


    public enum OwnedDollStatus
    {
        Inactive = 0,     // Không hoạt động
        Active = 1,       // Đang hoạt động
    }


    public enum DollCharacterLinkStatus
    {
        Inactive = 0,     // Không hoạt động
        Active = 1,       // Đang hoạt động
        Unbound = 2       // Đã ngắt kết nối
    }


    public enum UserStatus
    {
        Inactive = 0,     // Không hoạt động
        Active = 1,       // Đang hoạt động
    }
}

