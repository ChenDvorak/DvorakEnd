using System.ComponentModel.DataAnnotations;

namespace DvorakEnd.EntityFramework;
public class Blog
{
    public Guid Id { get; set; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "标题不能为空")]
    public string Title { get; set; } = "";
    [Required(AllowEmptyStrings = false, ErrorMessage = "分类不能为空")]
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    [Required(AllowEmptyStrings = false, ErrorMessage = "内容不能为空")]
    public string Body { get; set; } = "";

    /// <summary>
    /// UTC Date Time
    /// </summary>
    /// <value></value>
    public string CreateAt { get; set; } = DateTimeOffset.Now.ToString();
    /// <summary>
    /// UTC Date Time
    /// </summary>
    /// <value></value>
    public string UpdateAt { get; set; } = DateTimeOffset.Now.ToString();

}