namespace JobLessonASPNETMVCCore08v01.Services
{
    /// <summary>
    /// Интерфейс, описывающий механизм генерации отчета
    /// </summary>
    public interface IProductReport
    {
        string CatalogName { get; set; }
        string CatalogDescription { get; set; }
        DateTime CreationDate { get; set; }

        IEnumerable<(int id, string name, string category, decimal price)> Products { get; set; }

        FileInfo Create(string reportTemplateFile);
    }
}
