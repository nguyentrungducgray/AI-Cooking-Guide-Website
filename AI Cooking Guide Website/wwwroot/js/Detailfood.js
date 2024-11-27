@model AI_Cooking_Guide_Website.Models.ProfileViewModel

    < body >
    <div class="profile-container">
        <div class="profile-card">
            <h2 class="section-title">Thông tin cá nhân</h2>
            <div class="profile-details">
                <p><strong>Username:</strong> @Model.User.UserName</p>
                <p><strong>Email:</strong> @Model.User.Email</p>
                <div class="form-group">
                    <label for="PhoneNumber"><strong>Số điện thoại:</strong></label>
                    <input type="text" id="PhoneNumber" name="PhoneNumber" value="@Model.User.PhoneNumber" form="updateForm" class="input-field">
                </div>
                <div class="form-group">
                    <label for="Address"><strong>Địa chỉ:</strong></label>
                    <input type="text" id="Address" name="Address" value="@Model.User.Address" form="updateForm" class="input-field">
                </div>
            </div>

            <form id="updateForm" asp-action="UpdateProfile" method="post">
                <button type="submit" class="btn btn-primary custom-button">Lưu thay đổi</button>
            </form>

            <h2 class="section-title">Công thức của bạn:</h2>
            <div class="recipe-list">
                @if (Model.Recipes.Any())
                {
                    foreach(var recipe in Model.Recipes)
                {
                        <div class="recipe-item">
                            <img src="@recipe.Image" alt="@recipe.Name" class="recipe-image" />
                            <a href="@Url.Action("RecipeDetails", "Profile", new { id = recipe.Id })" class="recipe-name">@recipe.Name</a>
                            <p><strong>Ingredients:</strong> @string.Join(", ", recipe.Ingredients)</p>
                            <p><strong>Description:</strong> @recipe.Description</p>

                            <div class="status @(recipe.Status == "Approved" ? "status-approved" : recipe.Status == "Pending" ? "status-pending" : "status-rejected")">
                @recipe.Status
            </div>
        </div>
                    }
                }
        else
        {
            <p>Bạn chưa có công thức nào.</p>
        }
    </div>

@if (TempData["SuccessMessage"] != null) {
    <div class="success-message">
        @TempData["SuccessMessage"]
    </div>
}
        </div >
    </div >
</body >
