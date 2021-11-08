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
    public class Tests
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
        public void Test1()
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
        public void Test2()
        {
            driver.FindElement(By.XPath("(//*[@class='dy--PopularCategoryMain__link'])[1]")).Click();
            driver.FindElement(By.XPath("(//*[@class='CatalogCategoryCard__link'])[1]")).Click();
            var firstItem = driver.FindElement(By.CssSelector(".ProductCardVerticalLayout .IconFont_cart_add"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView()", firstItem);
            Actions actions = new Actions(driver);
            actions.MoveToElement(firstItem).Build().Perform();
        }

            [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}
