using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class root : MonoBehaviour
{

  /* Create a Root Object to store the returned json data in */
  [System.Serializable]
  public class Quotes
  {
    public Quote[] values;
  }

  [System.Serializable]
  public class PolicyHolder
  {
    public string policyholder_id;
  }

  [System.Serializable]
  public class Quote
  {
    public string package_name;
    public string sum_assured;
    public int base_premium;
    public string suggested_premium;
    public string created_at;
    public string quote_package_id;
    public QuoteModule module;
  }

  [System.Serializable]
  public class QuoteModule
  {
    public string type;
    public string make;
    public string model;
  }

  [Serializable]
  public class Param
  {
    public Param (string _key, string _value) {
      key = _key;
      value = _value;
    }
    public string key;
    public string value;
  }

  public string api_key = "";
  public TextMesh text_mesh;

  // Parameters for creating a policy holder
  private string firstName = "John";
  private string lastName = "DOe";
  private string email = "k@k.com";
  private string idNumber = "5610035014083";

  private void Start()
  {
    // CreateQuote("iPhone 6S 64GB LTE");
  }

  public void firstNameChanged(string newText) {
    firstName = newText;
  }

  public void lastNameChanged(string newText)
  {
    lastName = newText;
  }

  public void emailChanged(string newText)
  {
    email = newText;
  }

  public void idNumberChanged(string newText)
  {
    idNumber = newText;
  }

  public void CreatePolicyHolder() {
    List<Param> parameters = new List<Param>();
    parameters.Add(new Param("first_name", firstName));
    parameters.Add(new Param("last_name", lastName));
    parameters.Add(new Param("email", email));

    parameters.Add(new Param("id[type]", "id"));
    parameters.Add(new Param("id[number]", idNumber));
    parameters.Add(new Param("id[country]", "ZA"));

    StartCoroutine(CallCreatePolicyHolder("https://sandbox.root.co.za/v1/insurance/policyholders", parameters));
  }

  public void CreateQuote(string gadget) {
    List<Param> parameters = new List<Param>();
    parameters.Add(new Param("type", "root_gadgets"));
    parameters.Add(new Param("model_name", gadget));

    StartCoroutine(CallGetQuote("https://sandbox.root.co.za/v1/insurance/quotes", parameters));
  }

  IEnumerator CallGetQuote(String url, List<Param> parameters)
  {

    string auth = api_key + ":";
    auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
    auth = "Basic " + auth;

    WWWForm form = new WWWForm();

    foreach (var param in parameters) {
      form.AddField(param.key, param.value);
    }

    UnityWebRequest www = UnityWebRequest.Post(url, form);
    www.SetRequestHeader("AUTHORIZATION", auth);
    yield return www.Send();

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log(www.downloadHandler.text);
    }
    else
    {
      Quotes json = JsonUtility.FromJson<Quotes>("{\"values\":" + www.downloadHandler.text + "}");

      // Getting all packages
      
      if (json.values.Length > 0) {
        int limit = json.values.Length;
        Debug.Log(limit);

        // This is not working at the moment check out later
        foreach (var package in json.values) {
          string text = "";
          text += "Package Name " + package.package_name;
          text += "\nMake " + package.module.make + " Model: " + package.module.model;
          text += "\nPremium: R " + (package.base_premium / 100);
          text += "\n\n";
          Debug.Log("+++++");
          Debug.Log(text);
          text_mesh.text = text;
        }
      } else {
        text_mesh.text = "There are no Packages for the product : Product X";
      }  
    }
    yield return true;
  }

  IEnumerator CallCreatePolicyHolder(String url, List<Param> parameters)
  {
    string auth = api_key + ":";
    auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
    auth = "Basic " + auth;

    var jsonString = "{\"id\": { \"type\": \"id\", \"number\": \"8808125800084\",\"country\": \"ZA\"},\"first_name\": \"Erlich\",\"last_name\": \"Bachman\",\"email\": \"louw+erlich@root.co.za\"}";

    Debug.Log("REQUEST" + jsonString);
    UnityWebRequest www = UnityWebRequest.Post(url, jsonString);
    www.SetRequestHeader("AUTHORIZATION", auth);
    www.SetRequestHeader("Content-Type", "application/json");
    yield return www.Send();

    Debug.Log(www.isNetworkError);
    Debug.Log(www.isHttpError);

    if (www.isNetworkError || www.isHttpError)
    {
      Debug.Log("ERROR");
      Debug.Log(www.error);
      Debug.Log(www.responseCode);
      Debug.Log(www.downloadHandler.data);
    }
    else
    {
      Debug.Log("RESPONSE");
      
      PolicyHolder json = JsonUtility.FromJson<PolicyHolder>("{\"values\":" + www.downloadHandler.data + "}");

      // Getting policyHolder response
      
      Debug.Log("RESPONSE" + json);

    }
    yield return true;
  }
}
