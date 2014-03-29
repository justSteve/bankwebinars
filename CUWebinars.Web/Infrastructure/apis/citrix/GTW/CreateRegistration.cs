using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace CUWebinars.Web.Infrastructure.apis.citrix.GTW
{
    public class CreateRegistration
    {
        string orgKey = "901873";
        string access_token = "JIOHRkkCvmIKDY8QO0S4msbYH48N";
        public void Create_GTW_Registrant(string firstname, string lastname, string email, int idWebinar)
        {
            string url = "https://api.citrixonline.com/G2W/rest/organizers/" + orgKey + "/webinars/" + idWebinar + "/registrants";

            
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Headers.Add("Accept", "application/json");
            httpWebRequest.Headers.Add("Accept", "application/vnd.citrix.g2wapi-v1.1+json");
            httpWebRequest.Headers.Add("Authorization", "OAuth oauth_token=" + access_token);
            httpWebRequest.Method = "POST";

            string postData = "";

            byte[] requestBytes = Encoding.UTF8.GetBytes(postData);

            httpWebRequest.ContentLength = requestBytes.Length;

            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }

        }
    }
}


//https://api.citrixonline.com/oauth/authorize?client_id=9PsShthdirIz4DvlLbXkzEEHdNBGL7o9&redirect_uri=https%3A%2F%2Fapi.citrixonline.com%2Foauth%2Faccess_token%3Fgrant_type=authorization_code%26client_id=9PsShthdirIz4DvlLbXkzEEHdNBGL7o9
//function api_create_registrant($firstname,$lastname,$email,$phone,$jobtitle) {
//    global $org_key, $webinarid;
//    $organizerid = $org_key;
//    $url = "https://api.citrixonline.com/G2W/rest/organizers/".$org_key."/webinars/".$webinarid."/registrants";
//    $userArray = array(
//        "firstName" =>  $firstname,
//        "lastName" => $lastname,
//        "email" => $email,
//        "phone" => $phone,
//        "jobTitle" => $jobtitle
//    );
//    $data = apipost($userArray,$url);
//    if (!$data)
//        return false;
//    $registered = $org_key;
//    return ($registered);

//}


//function apipost($apiData,$URL) {
//    $data_json = json_encode ($apiData);
//    $options = array (CURLOPT_RETURNTRANSFER => true,
//    CURLOPT_HEADER => false, 
//    CURLOPT_FOLLOWLOCATION => true,
//    CURLOPT_ENCODING => "utf-8", 
//    CURLOPT_AUTOREFERER => true,
//    CURLOPT_CONNECTTIMEOUT => 120,
//    CURLOPT_TIMEOUT => 120,
//    CURLOPT_MAXREDIRS => 10, 
//    CURLOPT_HTTPHEADER => getJsonHeaders(),
//    CURLOPT_POSTFIELDS => $data_json
//    ); 
//    $ch = curl_init ( $URL );
//    curl_setopt_array ( $ch, $options );
//    $result = curl_exec ( $ch );
//    $err = curl_errno ( $ch );
//    $errmsg = curl_error ( $ch );
//    $header = curl_getinfo ( $ch );
//    $httpCode = curl_getinfo ( $ch, CURLINFO_HTTP_CODE );

//    if ($result) {
//        curl_close($ch);
//        $data = json_decode($result, true);
//        if ($data === false) return (false);
//        return($data);
//    } else {
//        curl_close($ch);
//        return false;        
//    }
//}

//               api_create_registrant($_POST['fname'],$_POST['lname'],$_POST['email'],$phone,$_POST['jobtitle']);



