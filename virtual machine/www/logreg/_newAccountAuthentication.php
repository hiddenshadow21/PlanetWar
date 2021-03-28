<?php

function SendAuthenticationMail($username, $email, $code)
{
	require "/var/www/mail/sendMail.php";

	$subject = "PlanetWar - new account verification";
	$body = "Hi ".$username.".";
	$body .= "\r\n Use the link below to verify your new account and start enjoying PlanetWar!";
	$body .= "\r\n\r\nhttp://40.69.215.163/logreg/newAccountVerify.php?email=".$email."&code=".$code;
	
	$response = SendMail($email, $subject, $body);
	
	return($response);
}

?>
