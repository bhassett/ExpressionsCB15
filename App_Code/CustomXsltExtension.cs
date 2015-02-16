// ------------------------------------------------------------------------------------------
// Licensed by Interprise Solutions.
// http://www.InterpriseSolutions.com
// For details on this license please visit  the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT.
// ------------------------------------------------------------------------------------------

using System;

/// <summary>
/// Use this class for custom XMLPackage Extension Functions.
/// *********************************************************
/// How to call this extension function from XMLPackage:
/// 1. Uncomment <!--<add name="custom" namespace="urn:custom" type="CustomXsltExtension, app_code"></add>--> on web.config
/// 2. Open any XMLPackage, find the code <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ise="urn:ise" exclude-result-prefixes="ise">
/// 3. Replace the code with <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ise="urn:ise" xmlns:custom="urn:custom" exclude-result-prefixes="ise">
/// 4. To call HelloWorld() on XMLPackage use this code <xsl:value-of select="custom:HelloWorld()" disable-output-escaping="yes" /> 
/// 5. Open browser and browse the page where the XMLPackage is called.
/// </summary>
public class CustomXsltExtension
{
	public CustomXsltExtension()
	{
	
	}

    /// <summary>
    /// Sample Extension Function
    /// </summary>
    /// <returns>Hellow World</returns>
    public string HelloWorld()
    {
        return "Hello World";
    }
}