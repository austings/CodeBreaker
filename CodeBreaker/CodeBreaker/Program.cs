using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1
{
    class CodeBreaker
    {
        /*
         * breaks the code at hack.ainfosec.com
         */
        static void Main(string[] args)
        {

            //Establish Driver
            IWebDriver driver = new ChromeDriver("C:\\Users\\Austin\\source\\repos\\ConsoleApp1\\ConsoleApp1\\bin\\Debug");
            driver.Navigate().GoToUrl("https://hack.ainfosec.com/");
            Actions action = new Actions(driver);

            //try to enter a previously given ID number
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
            IWebElement idBox = driver.FindElement(By.XPath("/html/body/site-nav/div/nav/div/div/ul/li[2]/div/div/button"));
            idBox.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            idBox = driver.FindElement(By.XPath("/html/body/site-help-modal/div/div/div/div[2]/resume-session-form/div/input"));
            System.Threading.Thread.Sleep(1000);
            idBox.SendKeys("[enter id]");
            System.Threading.Thread.Sleep(1000);
            driver.FindElement(By.XPath("/html/body/site-help-modal/div/div/div/div[2]/resume-session-form/button")).Click();
            System.Threading.Thread.Sleep(5000);//refresh page
            driver.Navigate().Refresh();
            

            //navigate to the brute force exercise
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
            IWebElement crackIcon = driver.FindElement(By.XPath("//challenge-list[@id='hackerchallenge-challenge-list']/div[3]/div/challenge-card[2]/div/div/div/div[2]/div/button"));
            action.MoveToElement(crackIcon).Perform();
            crackIcon.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);

            //guesses is the char array
            char[] guesses = new char[7];
            for (int i = 0; i < 7; i++)
                guesses[i] = '0';
            int score = 0;
            int pos = 6;

            System.Threading.Thread.Sleep(1000);
            do
            {
                try
                {


                    //Click into the input field
                    IWebElement inputBox = driver.FindElement(By.XPath("/html/body/div[3]/div[4]/challenge-list/div[3]/div/challenge-card[2]/challenge-modal/div/div/div/div[2]/div[2]/div/input"));
                    IWebElement submit = driver.FindElement(By.XPath("/html/body/div[3]/div[4]/challenge-list/div[3]/div/challenge-card[2]/challenge-modal/div/div/div/div[3]/button"));
                    //send guess
                    string myCode = new string(guesses);
                    inputBox.SendKeys(myCode);
                    submit.Click();
                    //retrieve score
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
                    IWebElement wrongA = driver.FindElement(By.CssSelector(".mt-2:nth-child(3)"));
                    Regex rx = new Regex(@"\d+");
                    Match m = rx.Match(wrongA.Text.ToString());
                    int newScore = Int32.Parse(m.Value);
                    //compare score to previous
                    Console.WriteLine("Guess:"+ myCode + " New score:"+newScore+" Old score:"+score);
                    if (newScore > score)
                    {
                        //if we find a correct character, move to the next value
                        pos--;
                        score = newScore;
                    }
                    else
                    {
                        //already had the correct value
                        if (newScore < score)
                        {
                            guesses[pos] = (Char)(Convert.ToUInt16(guesses[pos]) - 1);
                            pos--;
                            newScore = score;
                        }
                        else//this is the wrong value
                        {
                            //cycle through alphanumeric values
                            guesses[pos] = (Char)(Convert.ToUInt16(guesses[pos]) + 1);
                            if (guesses[pos] == 58)
                                guesses[pos] = 'A';
                            if (guesses[pos] == 91)
                                guesses[pos] = 'a';
                            if (guesses[pos] == 123)
                                guesses[pos] = '0';
                        }
                    }
                    //close the popups
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);
                    IWebElement closeMe = driver.FindElement(By.CssSelector(".mt-2:nth-child(3) > .close"));
                    closeMe.Click();
                    closeMe = driver.FindElement(By.CssSelector(".close:nth-child(1)"));
                    closeMe.Click();

                    System.Threading.Thread.Sleep(250);



                }
                catch (StaleElementReferenceException e)
                {
                    Console.WriteLine("Stale Element.. Retrying");
                }
                catch (ElementNotInteractableException e)
                {
                    Console.WriteLine("Element Not Interactable.. Retrying");
                }
                catch (WebDriverException e)
                {
                    Console.WriteLine("WebDriverException.. Retrying");
                }

            }
            while (pos!=-1);


        }
    }
}
