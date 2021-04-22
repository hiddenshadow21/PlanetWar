<?php

function SendAuthenticationMail($username, $email, $code)
{
	require "/var/www/mail/sendMail.php";

	$subject = "PlanetWar - new account verification";
	
	$body = "<h1 style='text-align:center; color: #6BECF7; font-size:5em;'>PlanetWar</h1>";
	$body .= "<h2 style='text-align:center; color: #6BECF7; font-size:3em;'>Hi ".$username.".</h2>";
	$body .= "<h3 style='text-align:center; font-size:1em;'>Use the link below to verify your new account and start enjoying PlanetWar!</h3>";
	$body .= "<h3 style='text-align:center; font-size:1em;'><a href='http://40.69.215.163/logreg/newAccountVerify.php?email=".$email."&code=".$code."'>Verify Your account</a></h3>";
		
	$response = SendMail($email, $subject, $body);
	
	return($response);
}

?>
