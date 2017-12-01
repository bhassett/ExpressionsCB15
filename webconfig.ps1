$path = $args[0]
$webConfig = $path+"\web.config"
$doc = new-object System.Xml.XmlDocument
$doc.Load($webConfig)
$doc.get_DocumentElement()."system.web".compilation.debug = "false"
$doc.Save($webConfig)