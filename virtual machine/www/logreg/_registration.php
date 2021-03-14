<?php

$email = "";
$password = "";
$username = "";
$code = "";

function DownloadDataFromUnity()
{
	global $email, $password, $username;

	if(isset($_POST["email"]))
	{
		$email = $_POST["email"];
	}
	else
	{
		return("R_DDFU_1");
	}
	
	if(isset($_POST["password"]))
	{
		$password = $_POST["password"];
	}
	else
	{
		return("R_DDFU_2");
	}
	
	if(isset($_POST["username"]))
	{
		$username = $_POST["username"];
	}
	else
	{
		return("R_DDFU_3");
	}
	
	return("_SUCCESSFUL");
}

function CreateNewAccount()
{
	require "/var/www/logreg/_init.php";
	global $username, $email, $password, $code;

	$emailRequest = "SELECT email FROM user WHERE email = '".$email."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if(mysqli_num_rows($results) == 0)
    	{
		$usernameRequest = "SELECT username FROM user WHERE username = '".$username."';";
		$results = mysqli_query($CONNECTION, $usernameRequest);

		if(mysqli_num_rows($results) == 0)
		{
			$code = md5(rand(0, 1000));
			$inputQuery = "INSERT INTO user (username, password, email, code) VALUES ('".$username."', '".$password."', '".$email."', '".$code."')";
			if(mysqli_query($CONNECTION, $inputQuery)) 
			{
				return("_SUCCESSFUL");
			} 
			return("R_CNA_1"); 
		} 
		return("R_CNA_2"); 
	} 
	return("R_CNA_3");
}

function SendVerificationMail()
{
	require "/var/www/logreg/_newAccountAuthentication.php";
	global $username, $email, $code;

	$response = SendAuthenticationMail($username, $email, $code);
				
	if($response != "SM_SM_SUCCESSFUL_1")
	{
		return($response);
	}
	return("R_CNA_SUCCESSFUL");		
}

function DeleteNewAccount()
{
	require "/var/www/logreg/_init.php";
	global $email;

	$deleteQuery = "DELETE FROM user WHERE email = '".$email."'";
					
	if(mysqli_query($CONNECTION, $deleteQuery))
	{
		return("_SUCCESSFUL");
	}
	return("R_DNA_1");
}

?>
