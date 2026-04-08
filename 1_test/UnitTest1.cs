using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace staff_tests;

public class Tests
{
    private IWebDriver driver;
    private WebDriverWait wait;

    // Введите данные своей УЗ
    private string loginValue = "your_login";
    private string passwordValue = "your_password";

    [SetUp]
    public void Setup()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); // неявное ожидание
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3)); // явное ожидание
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }

    public void Authorize()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");

        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys(loginValue);

        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys(passwordValue);

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']"))); // явное ожидание

        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.That(titlePageElement.Text, Does.Contain("Новости"),
            "После успешной авторизации должен открыться раздел 'Новости'.");
    }

    [Test]
    public void Authorization()
    {
        // 1. Выполнить авторизацию
        Authorize();

        // 2. Проверить, что открылась страница "Новости"
        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.That(titlePageElement.Text, Does.Contain("Новости"),
            "После авторизации заголовок страницы должен содержать 'Новости'.");
    }

    [Test]
    public void WrongPasswordErrorMessage()
    {
        // 1. Перейти на страницу авторизации
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");

        // 2. Найти поле логина
        var login = driver.FindElement(By.Id("Username"));

        // 3. Ввести существующий логин
        login.SendKeys(loginValue);

        // 4. Найти поле пароля
        var password = driver.FindElement(By.Id("Password"));

        // 5. Ввести неверный пароль
        password.SendKeys("WrongPassword");

        // 6. Найти кнопку "Войти"
        var enter = driver.FindElement(By.Name("button"));

        // 7. Нажать на кнопку "Войти"
        enter.Click();

        // 8. Дождаться появления сообщения об ошибке
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.validation-summary-errors"))); // явное ожидание

        // 9. Проверить текст ошибки
        var error = driver.FindElement(By.CssSelector("div.validation-summary-errors"));

        Assert.That(error.Text, Does.Contain("Неверный логин или пароль"),
            "При вводе неверного пароля должно появиться сообщение 'Неверный логин или пароль'.");
    }

    [Test]
    public void OpenProfilePage()
    {
        // 1. Выполнить авторизацию
        Authorize();

        // 2. Найти кнопку профиля
        var profileButton = driver.FindElements(By.CssSelector("[data-tid='ProfileMenu'] button"))
            .First(element => element.Displayed);

        // 3. Нажать на кнопку профиля
        profileButton.Click();

        // 4. Дождаться появления пункта "Мой профиль"
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Profile']"))); // явное ожидание

        // 5. Нажать на пункт "Мой профиль"
        var myProfile = driver.FindElement(By.CssSelector("[data-tid='Profile']"));
        myProfile.Click();

        // 6. Дождаться открытия страницы профиля
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='EmployeeName']"))); // явное ожидание

        // 7. Проверить, что страница профиля открыта
        var employeeName = driver.FindElement(By.CssSelector("[data-tid='EmployeeName']"));

        Assert.That(employeeName.Displayed, Is.True,
            "После нажатия на пункт 'Мой профиль' должна открыться страница профиля пользователя.");
    }

    [Test]
    public void ShowLoggedOutMessage()
    {
        // 1. Выполнить авторизацию
        Authorize();

        // 2. Найти кнопку бокового меню
        var sidebarButton = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));

        // 3. Нажать на кнопку бокового меню
        sidebarButton.Click();

        // 4. Дождаться открытия бокового меню
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SidePage__root']"))); // явное ожидание

        // 5. Найти кнопку "Выйти"
        var sideMenu = driver.FindElement(By.CssSelector("[data-tid='SidePage__root']"));
        var logoutButton = sideMenu.FindElement(By.CssSelector("[data-tid='LogoutButton']"));

        // 6. Нажать на кнопку "Выйти"
        logoutButton.Click();

        // 7. Дождаться появления сообщения о выходе
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".body-wrapper"))); // явное ожидание

        // 8. Проверить сообщение о выходе
        var logoutMessage = driver.FindElement(By.CssSelector(".body-wrapper"));

        Assert.That(logoutMessage.Text, Does.Contain("Вы вышли из учетной записи"),
            "После выхода из аккаунта должно появиться сообщение 'Вы вышли из учетной записи'.");
    }

    [Test]
    public void OpenChatWithUser()
    {
        // 1. Выполнить авторизацию
        Authorize();

        // 2. Перейти сразу на страницу диалогов
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/messages");

        // 3. Дождаться загрузки страницы диалогов
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='PageBody']"))); // явное ожидание

        // 4. Найти строку поиска на странице диалогов
        var pageBody = driver.FindElement(By.CssSelector("[data-tid='PageBody']"));
        var searchBar = pageBody.FindElements(By.CssSelector("[data-tid='SearchBar']"))
            .First(element => element.Displayed);

        // 5. Нажать на строку поиска
        searchBar.Click();

        // 6. Ввести в неё "User"
        var actions = new Actions(driver);
        actions.SendKeys("User").Perform();

        // 7. Дождаться появления пользователя User в результатах поиска
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ComboBoxMenu__item']"))); // явное ожидание

        // 8. Найти пользователя User в результатах поиска
        var user = driver.FindElements(By.CssSelector("[data-tid='ComboBoxMenu__item']"))
            .First(element => element.Displayed && element.Text.Contains("User"));

        // 9. Нажать на пользователя User
        user.Click();

        // 10. Дождаться появления поля ввода сообщения
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("textarea[placeholder='Напишите сообщение...']"))); // явное ожидание

        // 11. Проверить, что поле ввода сообщения отображается
        var messageInput = driver.FindElement(By.CssSelector("textarea[placeholder='Напишите сообщение...']"));

        Assert.That(messageInput.Displayed, Is.True,
            "После открытия диалога с User должно отображаться поле ввода сообщения.");
    }

    [Test]
    public void OpenCreateCommunityModalWindow()
    {
        // 1. Выполнить авторизацию
        Authorize();

        // 2. Перейти сразу на страницу сообществ
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

        // 3. Дождаться загрузки страницы сообществ
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']"))); // явное ожидание

        // 4. Найти кнопку "Создать"
        var createButton = driver.FindElements(By.TagName("button"))
            .First(element => element.Displayed && element.Text.Contains("СОЗДАТЬ"));

        // 5. Нажать на кнопку "Создать"
        createButton.Click();

        // 6. Дождаться появления поля "Название сообщества"
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("textarea[placeholder='Название сообщества']"))); // явное ожидание

        // 7. Проверить, что открылось окно создания сообщества
        var communityNameInput = driver.FindElement(By.CssSelector("textarea[placeholder='Название сообщества']"));

        Assert.That(communityNameInput.Displayed, Is.True,
            "После нажатия на кнопку 'Создать' должно открыться окно создания сообщества.");
    }
}