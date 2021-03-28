<?php

$email = "";
$password = "";
$code = "";

function DownloadDataFromUnity()
{
	global $email, $password, $code;

	if(isset($_POST["email"]))
	{
		$email = $_POST["email"];
	}
	else
	{
		return("NPV_DDFU_1");
	}

	if(isset($_POST["password"]))
	{
		$password = $_POST["password"];
	}
	else
	{
		return("NPV_DDFU_2");
	}

	if(isset($_POST["code"]))
	{
		$code = $_POST["code"];
	}
	else
	{
		return("NPV_DDFU_3");
	}
	
	return("_SUCCESSFUL");
}

function SetPassword()
{
	require "/var/www/logreg/_init.php";
	global $email, $password, $code;

	$emailRequest = "SELECT username FROM user WHERE email = '".$email."' AND code = '".$code."';";
	$results = mysqli_query($CONNECTION, $emailRequest);

	if($results == false)
	{
		mysqli_close($CONNECTION);
		return("NPV_SP_1");
	}

	if(mysqli_num_rows($results) == 1)
	{
		$inputQuery = "UPDATE user SET password = '".$password."' 
			WHERE email = '".$email."' AND code = '".$code."';";

		if(mysqli_query($CONNECTION, $inputQuery))
		{
			mysqli_close($CONNECTION);
			return("NPV_SP_SUCCESSFUL");
		}

		mysqli_close($CONNECTION);
		return("NPV_SP_3");
	} 
	mysqli_close($CONNECTION);
	return("NPV_SP_2");
}

?>
