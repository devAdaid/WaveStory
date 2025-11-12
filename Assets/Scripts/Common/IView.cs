public interface IView<T> where T : IPresenter
{
    void SetPresenter(T presenter);
}
