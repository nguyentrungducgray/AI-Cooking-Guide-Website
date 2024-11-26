namespace AI_Cooking_Guide_Website.FirebaseStorage
{
    public class FirebaseStorageService
    {
        private static readonly string FirebaseStorageUrl = "https://firebasestorage.googleapis.com/v0/b/YOUR_PROJECT_ID.appspot.com/o/";

        public async Task UploadFileAsync(string filePath, string fileName)
        {
            using (var client = new HttpClient())
            {
                var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                var formContent = new MultipartFormDataContent();
                formContent.Add(fileContent, "file", fileName);

                var response = await client.PostAsync(FirebaseStorageUrl + fileName + "?uploadType=multipart", formContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File uploaded successfully.");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);  // In ra thông tin tệp đã tải lên
                }
                else
                {
                    Console.WriteLine("Error uploading file.");
                }
            }
        }
    }
}
