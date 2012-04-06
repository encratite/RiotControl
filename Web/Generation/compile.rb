#This script makes a single big JavaScript file out of all the components in the Module directory.

def joinPaths(*arguments)
  separator = '/'
  expression = Regexp.new "\\#{separator}+"
  path = arguments.join(separator).gsub(expression, separator)
  return path
end

def readDirectory(path)
  begin
    data = Dir.entries(path)
    data.reject! do |entry|
      ['.', '..'].include? entry
    end

    output = []
    data.each do |entry|
      output << joinPaths(path, entry)
    end
    return output
  rescue Errno::ENOENT
    raise "Unable to read #{path}"
  end
end

def readFile(path)
    begin
      file = File.open(path, 'rb')
      output = file.read
      file.close
      return output
    rescue Errno::ENOENT
      raise "Unable to open file #{path}"
    end
  end

def readLines(path)
    data = readFile path
    data = data.gsub("\r", '')
    return data.split "\n"
  end

def writeFile(path, data)
  file = File.open(path, 'wb+')
  file.write data
  file.close
end

def compile(path, outputFile)
  targets = readDirectory(path)
  output = ''
  comment = '//'
  targets.each do |target|
    lines = readLines(target)
    lines.each do |line|
      line = line.strip
      if line.empty?
        next
      end
      if line.size >= comment.size && line[0..comment.size - 1] == comment
        next
      end
      output += "#{line}\n"
    end
  end
  writeFile(outputFile, output)
end

compile('../Script/Module', '../Script/RiotControl.js')
