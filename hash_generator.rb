# hash_generator.rb — Ruby версия

require 'digest'
require 'optparse'
require 'base64'

options = {}
OptionParser.new do |opts|
  opts.banner = "Usage: ruby hash_generator.rb [options] [input]"
  opts.on("-f", "--format FORMAT", "Формат (hex, base64)") { |f| options[:format] = f }
  opts.on("-F", "--file FILE", "Файл для хеширования") { |f| options[:file] = f }
  opts.on("-o", "--output FILE", "Сохранить хеш в файл") { |o| options[:output] = o }
  opts.on("-c", "--compare HASH", "Сравнить с эталонным хешем") { |c| options[:compare] = c }
end.parse!

format = options[:format] || 'hex'
file_path = options[:file]
output = options[:output]
compare = options[:compare]
input = ARGV[0]

puts "\e[36m🔐 Hash Generator (MD5) (Ruby)\e[0m"
puts "Алгоритм: MD5"

def compute_hash(data)
  Digest::MD5.hexdigest(data)
end

def hash_file(filename)
  Digest::MD5.file(filename).hexdigest
end

hex_digest = if file_path
  puts "📂 Хеширование файла: #{file_path}"
  hash_file(file_path)
elsif input
  puts "📝 Входные данные: #{input}"
  compute_hash(input)
else
  puts "📝 Чтение из STDIN (Ctrl+D для окончания)"
  data = STDIN.read
  if data.empty?
    puts "\e[33m⚠️ Пустой ввод.\e[0m"
    exit 1
  end
  compute_hash(data)
end

result = if format == 'base64'
  Base64.strict_encode64([hex_digest].pack('H*'))
else
  hex_digest
end

puts "\e[32mХеш (#{format}):\e[0m"
puts result

if compare
  if result == compare
    puts "\e[32m✅ Хеши совпадают!\e[0m"
  else
    puts "\e[31m❌ Хеши не совпадают!\e[0m"
  end
end

if output
  File.write(output, result + "\n")
  puts "\e[32m💾 Сохранено в #{output}\e[0m"
end
