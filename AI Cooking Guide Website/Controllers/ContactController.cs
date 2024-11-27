using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using AI_Cooking_Guide_Website.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class ContactController : Controller
    {
        [Authorize] // Chỉ cho phép người dùng đã đăng nhập
        public IActionResult Index()
        {
            // Lấy thông tin họ và tên, email từ hồ sơ cá nhân
            var fullName = User.Identity.Name; // Họ và tên từ User.Identity
            var email = User.FindFirst("email")?.Value; // Email từ Claim "email"

            var contact = new ContactModel
            {
                FullName = fullName,
                Email = email
            };

            return View(contact);
        }

        // POST: Contact/SendMessage
        [HttpPost]
        [Authorize] // Chỉ người dùng đăng nhập mới gửi tin nhắn
        public IActionResult SendMessage(ContactModel contact)
        {
            // Đọc dữ liệu hiện tại từ contact.json trong thư mục Data
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "contact.json");
            var contacts = new List<ContactModel>();

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);

                // Kiểm tra nếu dữ liệu là đối tượng duy nhất thì tạo một mảng chứa nó
                if (!string.IsNullOrEmpty(json))
                {
                    if (json.StartsWith("{")) // Dữ liệu là đối tượng, không phải mảng
                    {
                        var singleContact = JsonConvert.DeserializeObject<ContactModel>(json);
                        contacts.Add(singleContact); // Thêm vào danh sách
                    }
                    else // Dữ liệu là mảng
                    {
                        contacts = JsonConvert.DeserializeObject<List<ContactModel>>(json);
                    }
                }
            }
            // Tự động gán Id cho tin nhắn mới
            if (contacts.Count > 0)
            {
                contact.Id = contacts.Max(c => c.Id) + 1; // Lấy Id lớn nhất và cộng thêm 1
            }
            else
            {
                contact.Id = 1; // Nếu chưa có tin nhắn nào, gán Id là 1
            }

            // Thêm đối tượng Contact vào danh sách và ghi lại vào contact.json
            contact.DateSent = DateTime.Now; // Đặt thời gian gửi tin nhắn
            contacts.Add(contact);

            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, updatedJson);


            // Sử dụng TempData để truyền thông báo đến view
            TempData["SuccessMessage"] = "Gửi tin nhắn thành công! Cảm ơn bạn đã liên hệ.";

            // Chuyển hướng đến trang Liên hệ sau khi gửi thành công
            return RedirectToAction("Index");
        }

    }
}
