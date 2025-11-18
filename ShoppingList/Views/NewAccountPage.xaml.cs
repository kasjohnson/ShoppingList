using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShoppingList.Models;
using System.Text.RegularExpressions;

namespace ShoppingList.Views;

public partial class NewAccountPage : ContentPage
{
    public NewAccountPage()
    {
        InitializeComponent();
        Title = "Create New Account";
    }

    async void CreateAccount_OnClicked(object sender, EventArgs e)
    {
        //Do Passwords Match
        if (txtPassword1?.Text != txtPassword2?.Text)
        {
            await DisplayAlert("Error", "Passwords don't match.", "OK");
            return;
        }
        
        //Is a Valid email address = @ . (in this order) -substring or
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (txtEmail.Text== null || !Regex.IsMatch((txtEmail.Text), pattern))
        {
            await DisplayAlert("Error", "Please enter a valid email address.", "OK");
            return;
        }
        
        
        //api stuff
        var data = JsonConvert.SerializeObject(new UserAccount(txtUser.Text, txtPassword1.Text, txtEmail.Text));

        var client = new HttpClient();
        var response = await client.PostAsync(new Uri("https://joewetzel.com/fvtc/account/createuser"),
            new StringContent(data, Encoding.UTF8, "application/json"));

        var AccountStatus = response.Content.ReadAsStringAsync().Result;

        
        //does the user exist?
        if (AccountStatus == "user exists")
        {
            await DisplayAlert("Error", "Sorry, this username has been taken!", "OK");
            return;
        }
        
        //is the email in use? been used before?
        if (AccountStatus == "email exists")
        {
            await DisplayAlert("Error", "Sorry, this email has already been used!", "OK");
            return;
        }
        
        if (AccountStatus == "complete")
        {
            response = await client.PostAsync(new Uri("https://joewetzel.com/fvtc/account/login"),
                new StringContent(data, Encoding.UTF8, "application/json"));

            var SKey = response.Content.ReadAsStringAsync().Result;
            
            if (!string.IsNullOrEmpty(SKey) && SKey.Length< 50)
            {
                App.SessionKey = SKey;
                Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "Sorry, there was an issue logging you in.", "OK");
                return;
            }
            
            
        }
        else
        {
            await DisplayAlert("Error", "Sorry there was an error creating your account!", "OK");
            return;
        }
        
        
    }
}