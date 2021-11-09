using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTests
{
    public class CitilinkTests
    {
        WebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.citilink.ru/");
            driver.FindElement(By.XPath("//button[contains(.,'Я согласен')]")).Click();
        }

        [Test]
        public void TestPriceFilter()
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(x => driver.FindElement(By.CssSelector(".dy--PopularCategoryMain__link")).Displayed);
            //((IJavaScriptExecutor)driver).ExecuteScript("document.getElementsByClassName('dy--PopularCategoryMain__link')[0].scrollIntoView()");
            driver.FindElement(By.XPath("(//*[@class='dy--PopularCategoryMain__link'])[1]")).Click();
            driver.FindElement(By.XPath("(//*[@class='CatalogCategoryCard__link'])[1]")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(x => driver.FindElement(By.XPath("//*[@data-meta-name='FiltersLayout']")).Enabled);
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@name='input-min']")).Clear();
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@name='input-min']")).SendKeys("1000");

            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@name='input-max']")).Clear();
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@name='input-max']")).SendKeys("10000");
            driver.FindElement(By.XPath("//*[@data-meta-name='FilterListGroupsLayout']//input[@name='input-max']")).SendKeys(Keys.Enter);

            new WebDriverWait(driver, TimeSpan.FromSeconds(5))
                .Until(x => driver.FindElements(By.CssSelector(".StickyOverlayLoader__overlay")).Count > 0);

            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => driver.FindElements(By.CssSelector(".StickyOverlayLoader__overlay")).Count == 0);

            var webPrices = driver.FindElements(By.CssSelector(".ProductCardVerticalLayout__wrapper-cart .ProductCardVerticalPrice__price-current_current-price"));

            int[] actualPrices = webPrices.Select(webPrice => Int32.Parse(webPrice.Text.Trim())).ToArray();
            actualPrices.ToList().ForEach(price => Assert.IsTrue(price >= 1000 && price <= 10000));
        }

        [Test]
        public void TestAddToCartButtonTooltipText()
        {
            var firstButtonAddToCart = driver.FindElement(By.XPath("//*[contains(@class,' ProductCardVerticalCart__button-add ')]/.."));
            new Actions(driver).MoveToElement(firstButtonAddToCart.FindElement(By.CssSelector(".IconFont_cart_add"))).Build().Perform();
            Assert.IsTrue(firstButtonAddToCart.FindElements(By.XPath(".//*[contains(@class,'Hint__block_active')]")).Any(),
                "Tooltip on 'Add item to card' has not appeared");
            Assert.AreEqual(firstButtonAddToCart.FindElement(By.XPath(".//*[contains(@class,'Hint__block_active')]")).Text.Trim(), "Добавить в корзину",
                "Incorrect tooltip text");
        }

        [Test]
        public void NegativeTestPhoneNumberConfirmationWithEmptyPhoneNumber()
        {
            driver.FindElement(By.XPath("//*[contains(@class,'IconAndTextWithCount__text_mainHeader') and normalize-space()='Войти']")).Click();
            driver.FindElement(By.CssSelector(".js--AuthGroup__tab-sign-up")).Click();
            driver.FindElement(By.CssSelector(".js--SignUp__input-name__container-input")).SendKeys("Test");
            driver.FindElement(By.CssSelector(".js--SignUp__input-email__container-input")).SendKeys("shjfgshjds534.yhft@mail.ru");
            driver.FindElement(By.CssSelector(".js--SignUp__button-confirm-phone")).Click();
            Assert.IsTrue(!driver.FindElements(By.XPath("//button[contains(@class,'js--SignUp__button-confirm-phone') and not(@disabled)]")).Any(),
                "Button 'Подтвердить номер телефона' is enabled when phone number input is empty");
        }

            [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}
