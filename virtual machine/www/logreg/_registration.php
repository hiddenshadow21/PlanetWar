<?php

$email = "";
$password = "";
$confPassword = "";
$username = "";
$code = "";

function DownloadDataFromUnity()
{
	global $email, $password, $confPassword, $username;

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
	
	if(isset($_POST["confPassword"]))
	{
		$confPassword = $_POST["confPassword"];
	}
	else
	{
		return("R_DDFU_3");
	}
	
	if(isset($_POST["username"]))
	{
		$username = $_POST["username"];
	}
	else
	{
		return("R_DDFU_4");
	}
	
	return("_SUCCESSFUL");
}

function UsernameTest()
{
	require "/var/www/logreg/_consts.php";
	global $username;

	if(strlen($username) > 20) 
	{ 
		return("R_UT_1"); 
	} 
	
	if(strpos($username, $RESTRICTED_MARK) == true)
	{
		return("R_UT_2"); 
	}	
	
	return("_SUCCESSFUL");
}

function EmailTest()
{
	require "/var/www/logreg/_consts.php";
	global $email;

	if(!filter_var($email, FILTER_VALIDATE_EMAIL))
    	{
		return("R_ET_1"); 
	}
	
	if(strlen($email) > 20) 
	{ 
		return("R_ET_2"); 
	}
	
	if(strpos($email, $RESTRICTED_MARK) == true)
    	{
		return("R_ET_3");
	}
	
	return("_SUCCESSFUL");
}

function PasswordTest()
{	
	require "/var/www/logreg/_consts.php";
	global $password, $confPassword;

	if(strlen($password) <= 7 || strlen($password) > 20) 
	{ 
		return("R_PT_1"); 
	}
	
	if(strcspn($password, '0123456789') == strlen($password))
	{ 
		return("R_PT_2"); 
	}
	
    	if(strpos($password, $RESTRICTED_MARK) == true)
	{
		return("R_PT_3");
	}
	
	if($password != $confPassword)
	{ 
		return("R_PT_4"); 
	}
	
	return("_SUCCESSFUL");
}


function CreateNewAccount()
{
	require "/var/www/logreg/_init.php";
	global $username, $email, $password, $confPassword, $code;

	$emailRequest = "SELECT email FROM user WHERE email = '".$email."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if(mysqli_num_rows($results) == 0)
    	{
		$usernameRequest = "SELECT username FROM user WHERE username = '".$username."';";
		$results = mysqli_query($CONNECTION, $usernameRequest);

		if(mysqli_num_rows($results) == 0)
		{
			$code = md5(rand(0, 1000));
			$inputQuery = "INSERT INTO user (username, passwd, email, code) VALUES ('".$username."', '".$password."', '".$email."', '".$code."')";
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

	$response = SendAuthenticationMail($username, $email, $code); //3-rd parameter to-do!! new function!
				
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
