using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	22.11.2019 - first
    31.01.2020 - rename to OpenURLButton
*/
namespace Mkey
{
	public class OpenURLButton : MonoBehaviour
	{
        [SerializeField]
        private string URL;

        public void Click()
        {
            if (!string.IsNullOrEmpty(URL)) Application.OpenURL(URL);
        }

        public void OpenGmailApp()
        {
            // Construct the mailto URL
            string email = URL; // Replace with the recipient's email address
            string subject = MyEscapeURL("Query");
            string body = MyEscapeURL("");

            string mailto = string.Format("mailto:{0}?subject={1}&body={2}", email, subject, body);

            // Open the Gmail app or default mail client
            Application.OpenURL(mailto);
        }

        // Helper function to escape URL components
        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }
    }
}
