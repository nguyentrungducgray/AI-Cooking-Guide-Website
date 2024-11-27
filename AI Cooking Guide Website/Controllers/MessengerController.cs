using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using AI_Cooking_Guide_Website.Models;

namespace AI_Cooking_Guide_Website.Controllers
{
    public class MessengerController : Controller
    {
        public IActionResult Index()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "contact.json");
            var contacts = new List<ContactModel>();

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    contacts = JsonConvert.DeserializeObject<List<ContactModel>>(json);
                }
            }

            return View(contacts);
        }

        public IActionResult Reply(int id)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "contact.json");
            var contacts = new List<ContactModel>();

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    contacts = JsonConvert.DeserializeObject<List<ContactModel>>(json);
                }
            }

            var contact = contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            var adminReplyModel = new AdminReplyModel
            {
                Id = contact.Id,
                UserName = contact.FullName,
                Message = contact.Message
            };

            return View(adminReplyModel);
        }

        [HttpPost]
        public IActionResult Reply(AdminReplyModel replyModel, string replyMessage)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "contact.json");
            var contacts = new List<ContactModel>();

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(json))
                {
                    contacts = JsonConvert.DeserializeObject<List<ContactModel>>(json);
                }
            }

            var contact = contacts.FirstOrDefault(c => c.Id == replyModel.Id);
            if (contact == null)
            {
                return NotFound();
            }

            var newReply = new AdminReplyModel
            {
                Id = contact.Id,
                UserName = contact.FullName,
                Message = replyMessage,
                DateSent = DateTime.Now
            };

            string replyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "replycontact.json");
            var replies = new List<AdminReplyModel>();

            if (System.IO.File.Exists(replyFilePath))
            {
                var replyJson = System.IO.File.ReadAllText(replyFilePath);
                if (!string.IsNullOrEmpty(replyJson))
                {
                    replies = JsonConvert.DeserializeObject<List<AdminReplyModel>>(replyJson);
                }
            }

            replies.Add(newReply);

            var updatedReplyJson = JsonConvert.SerializeObject(replies, Formatting.Indented);
            System.IO.File.WriteAllText(replyFilePath, updatedReplyJson);

            TempData["SuccessMessage"] = "Phản hồi đã được gửi thành công!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Correct the file path here
            var contacts = LoadContactsFromFile("Data/contact.json");

            if (contacts == null)
            {
                TempData["ErrorMessage"] = "Không thể tải dữ liệu liên hệ.";
                return RedirectToAction("Index");
            }

            var contact = contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                TempData["ErrorMessage"] = "Liên hệ không tồn tại.";
                return RedirectToAction("Index");
            }

            // Remove the contact
            contacts.Remove(contact);

            // Save the updated contacts list back to the file
            SaveContactsToFile("Data/contact.json", contacts);

            // Set success message and reload the page
            TempData["SuccessMessage"] = "Liên hệ đã được xóa thành công!";
            return RedirectToAction("Index");
        }

        private List<ContactModel> LoadContactsFromFile(string fileName)
        {
            try
            {
                // Ensure the path is correct
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                string json = System.IO.File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<ContactModel>>(json);
            }
            catch (Exception ex)
            {
                // Handle errors, log or show message if needed
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;
            }
        }

        private void SaveContactsToFile(string fileName, List<ContactModel> contacts)
        {
            try
            {
                // Ensure the path is correct
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
                System.IO.File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Handle errors, log or show message if needed
                Console.WriteLine($"Error writing file: {ex.Message}");
            }
        }

    }
}
