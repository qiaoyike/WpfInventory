namespace WpfInventory.App.ViewModels;

/// <summary>
/// 左侧导航菜单项。
/// 每个菜单项对应一个页面 ViewModel 类型，
/// 点击后由主窗口 ViewModel 创建新的 DI 作用域并解析该类型。
/// </summary>
public class NavigationItemViewModel
{
    public NavigationItemViewModel(string title, string icon, Type viewModelType)
    {
        Title = title;
        Icon = icon;
        ViewModelType = viewModelType;
    }

    /// <summary>
    /// 菜单标题。
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 菜单图标占位符（用简单字符表示，避免引入图片资源）。
    /// </summary>
    public string Icon { get; }

    /// <summary>
    /// 点击后要显示的页面 ViewModel 类型。
    /// </summary>
    public Type ViewModelType { get; }
}