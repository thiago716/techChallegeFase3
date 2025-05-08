namespace Application.ViewModel
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public List<string> Errors { get; set; } // opcional, caso queira tratar erros
    }
}
