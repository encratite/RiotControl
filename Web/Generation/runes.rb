require 'nil/file'
require 'nil/http'

class Rune
  attr_reader :id, :name, :description

  def initialize(id, name, description)
    @id = id
    @name = name
    @description = description
  end
end

def processTier(tier)
  host = 'http://na.leagueoflegends.com'
  url = "#{host}/runes/#{tier}"
  puts "Downloading tier #{tier}: #{url}"
  contents = Nil.httpDownload(url)
  if contents == nil
    raise 'Unable to download rune data'
  end
  pattern = /<td class="rune_type"><img src="(.+?(\d+)\.png)"><\/td>.*?<td class="rune_name">(.+?)<\/td>.*?<td class="rune_description">(.+?)<\/td>/m
  output = []
  contents.scan(pattern) do |match|
    path = match[0]
    id = match[1].to_i
    name = match[2]
    description = match[3]
    rune = Rune.new(id, name, description)
    output << rune

    imageURL = "#{host}#{path}"
    filename = "../Image/Rune/#{id}.png"
    if !File.exists?(filename)
      puts "Downloading rune image: #{imageURL}"
      download = Nil.httpDownload(imageURL)
      if download == nil
        raise "Unable to retrieve #{imageURL}"
      end
      Nil.writeFile(filename, download)
    end
  end
  return output
end

def javaScriptString(input)
  output = input.gsub('"', "\\\"")
  return "\"#{output}\""
end

runes = []
(1..3).each do |i|
  runes += processTier(i)
end
runes.sort! do |x, y|
  x.id <=> y.id
end

output = "function Rune(name, description)\n"
output += "{\n"
output += "    this.name = name;\n"
output += "    this.description = description;\n"
output += "}\n\n"

output += "function getRune(id)\n"
output += "{\n"
output += "    switch(id)\n"
output += "    {\n"
runes.each do |rune|
  output += "    case #{rune.id}:\n"
  output += "        return new Rune(#{javaScriptString(rune.name)}, #{javaScriptString(rune.description)});\n"
end
output += "    default:\n"
output += "        return new Rune('Unknown rune', 'Unknown');\n"
output += "    }\n"
output += "}"

Nil.writeFile('../Script/Module/Runes.js', output)
